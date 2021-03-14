using Statistician4Worker.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statistician4Worker
{
    public class TableConnector
    {
        #region TableConstructor
        private class TableConstructor
        {
            Func<DataTable> GetFunc { get; set; }
            string[] RenameColumns { get; set; }
            int SortNum { get; set; }

            public TableConstructor(Func<DataTable> func, int sort, params string[] renameColumns)
            {
                this.GetFunc = func;
                this.SortNum = sort;
                this.RenameColumns = renameColumns ?? new string[0];
            }

            public DataTable GetTable()
            {
                var t = this.GetFunc();
                t.TableName = $"t{ SortNum.ToString().PadLeft(3, '0') }";
                int n = (t.Columns.Count < RenameColumns.Length) ? t.Columns.Count : RenameColumns.Length;
                for (int i = 0; i < n; i++)
                    t.Columns[i].ColumnName = RenameColumns[i] ?? $"未命名{ t.TableName }{ i }";
                
                return t;
            }
        }
        #endregion

        #region 字段属性
        private int SortNum = 0;
        private string SeekColumnName;
        private IList<TableConstructor> TableConstructors = new List<TableConstructor>();
        #endregion

        #region 构造函数
        private TableConnector(Func<DataTable> mainTableHandler, params string[] renameColumns)
        {
            Func<DataTable> func = () =>
            {
                DataTable mainTable;

                mainTable = mainTableHandler() ?? new DataTable();
                if (mainTable.Columns.Count == 0) throw new Exception("联接主表没有数据列！");
                this.SeekColumnName = renameColumns?[0] ?? mainTable.Columns[0].ColumnName;

                return mainTable;
            };

            this.TableConstructors.Add(new TableConstructor(func, SortNum++, renameColumns));
        }

        private TableConnector(string seekColumnName, Func<IEnumerable> dataHandler)
        {
            Func<DataTable> func = () =>
            {
                DataTable mainTable = null;

                this.SeekColumnName = seekColumnName;
                mainTable = new DataTable();
                mainTable.Columns.Add(seekColumnName, typeof(string));

                var data = dataHandler();
                DataRow newRow = null;
                foreach (var item in data)
                {
                    newRow = mainTable.NewRow();
                    mainTable.Rows.Add(newRow);

                    newRow.SetField(seekColumnName, item?.ToString());
                }

                return mainTable;
            };

            this.TableConstructors.Add(new TableConstructor(func, SortNum++));
        }
        
        public static TableConnector NewJoin(Func<DataTable> mainTableHandler, params string[] renameColumns)
        {
            return new TableConnector(mainTableHandler, renameColumns);
        }

        public static TableConnector NewJoin(string seekColumnName, Func<IEnumerable> dataHandler)
        {
            return new TableConnector(seekColumnName, dataHandler);
        }

        #endregion

        #region 联接函数
        public TableConnector LeftJoin(TableConnector reportForm)
        {
            throw new NotImplementedException();
        }

        public TableConnector LeftJoin(Func<DataTable> handler, params string[] renameColumns)
        {
            this.TableConstructors.Add(new TableConstructor(handler, SortNum++, renameColumns));
            return this;
        }
        #endregion

        #region 生成函数

        public DataTable GenerateTable()
        {
            var tasks = new List<Task<DataTable>>();

            foreach (var constructor in TableConstructors)
            {
                tasks.Add(Task.Factory.StartNew(constructor.GetTable));
            }

            var finalTask = Task.Factory.ContinueWhenAll(tasks.ToArray(), results =>
            {
                var tables = results.Select(r => r.Result).OrderBy(t => t.TableName).ToArray();
                DataTable mergedTable = null;

                for (int i = 0; i < tables.Length; i++)
                {
                    if (mergedTable == null)
                    {
                        mergedTable = tables[i];
                        continue;
                    }

                    tables[i].Columns[0].ColumnName = this.SeekColumnName;
                    mergedTable = DataTableHelper.JoinTwoDataTablesOnOneColumn(mergedTable, tables[i], this.SeekColumnName, DataTableHelper.JoinType.Left);
                }

                return mergedTable;
            });

            finalTask.Wait();

            return finalTask.Result;
        }
        #endregion
    }
}
