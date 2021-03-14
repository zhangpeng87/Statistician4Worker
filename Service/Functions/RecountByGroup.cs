using Service.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Functions
{
    /// <summary>
    /// 对查询结果再统计。
    /// </summary>
    public class RecountByGroup : FileOfStatements
    {
        private FileOfStatements _file;

        private RecountByGroup(string filePath)
            : base(filePath)
        {

        }

        public RecountByGroup(FileOfStatements file)
            : base(file.FilePath)
        {
            this._file = file;
        }

        public override DataTable QueryResult()
        {
            var result = this._file.QueryResult();
            // 再统计各级
            DataSet set = new DataSet();
            set.Tables.Add(this.ComputeAllProjects(result));
            set.Tables.Add(this.ComputeByProject(result));
            set.Tables.Add(this.ComputeByCooperator(result));
            set.Tables.Add(this.ComputeByGroup(result));

            return set.MergeAllTables();
        }

        /// <summary>
        /// 按全场计算。
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private DataTable ComputeAllProjects(DataTable table)
        {
            int length = 4;

            var rows = table.AsEnumerable()
                         .Where(row => row.Field<string>(0).Length == length);

            DataTable result = null;
            if (rows.Any())
            {
                result = rows.CopyToDataTable();
            }
            else
            {
                result = table.Clone();
            }

            return result;
        }

        /// <summary>
        /// 按标段项目计算。
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        private DataTable ComputeByProject(DataTable table)
        {
            int length = 8;

            var rows = table.AsEnumerable()
                            .Where(row => row.Field<string>(0).Length >= length);
            DataTable result = null;
            if (rows.Any())
            {
                result = rows.GroupBy(row => row.Field<string>(0).Substring(0, length))
                            .Select(grp =>
                            {
                                var newRow = table.NewRow();

                                newRow[0] = grp.Key;
                                for (int i = 1; i < table.Columns.Count; i++)
                                    newRow[i] = grp.Sum(row => Convert.ToInt32(row[table.Columns[i].ColumnName]));

                                return newRow;
                            }).CopyToDataTable();
            }
            else
            {
                result = table.Clone();
            }

            return result;
        }

        /// <summary>
        /// 按参建单位计算。
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        private DataTable ComputeByCooperator(DataTable table)
        {
            int length = 12;

            var rows = table.AsEnumerable()
                            .Where(row => row.Field<string>(0).Length >= length);
            DataTable result = null;
            if (rows.Any())
            {
                result = rows.GroupBy(row => row.Field<string>(0).Substring(0, length))
                            .Select(grp =>
                            {
                                var newRow = table.NewRow();

                                newRow[0] = grp.Key;
                                for (int i = 1; i < table.Columns.Count; i++)
                                    newRow[i] = grp.Sum(row => Convert.ToInt32(row[table.Columns[i].ColumnName]));

                                return newRow;
                            }).CopyToDataTable();
            }
            else
            {
                result = table.Clone();
            }

            return result;
        }

        /// <summary>
        /// 按部门班组计算。
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        private DataTable ComputeByGroup(DataTable table)
        {
            int length = 16;

            var rows = table.AsEnumerable()
                         .Where(row => row.Field<string>(0).Length == length);

            DataTable result = null;
            if (rows.Any())
            {
                result = rows.CopyToDataTable();
            }
            else
            {
                result = table.Clone();
            }

            return result;
        }
    }
}
