using Statistician4Worker.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statistician4Worker
{
    public class ReportGenerator
    {
        protected DataTable ReportTable { get; private set; }
        protected ReportHeaderCollection Headers { get; private set; }
        public ReportGenerator(ReportHeaderCollection headers)
        {
            this.Headers = headers;
            this.ReportTable = new DataTable();
        }
        /// <summary>
        /// 构造报表结构。
        /// </summary>
        protected virtual void ConstructReport()
        {
            DataColumn column = null;
            foreach (var header in this.Headers.AllReportHeaders)
            {
                column = new DataColumn(header.HeaderText, typeof(string));
                this.ReportTable.Columns.Add(column);
            }
        }
        /// <summary>
        /// 加载查找列。
        /// </summary>
        protected virtual void LoadSeekColumn()
        {
            var data = this.Headers.SeekHeader.ValueHandler(this.Headers.SeekHeader.State);

            DataRow newRow = null;
            foreach (var item in data)
            {
                newRow = this.ReportTable.NewRow();
                this.ReportTable.Rows.Add(newRow);

                newRow.SetField(this.Headers.SeekHeader.HeaderText, item);
            }
        }
        /// <summary>
        /// 设置各列值。
        /// </summary>
        protected virtual void LoadComputedColumn()
        {
            foreach (var header in this.Headers.ComputedHeaders)
            {
                var data = header.ValueHandler(header.State);
                foreach (string key in data.Keys)
                {
                    this.ReportTable
                        .AsEnumerable()
                        .Single(r => r.Field<string>(this.Headers.SeekHeader.HeaderText).Equals(key))
                        .SetField(header.HeaderText, data[key]?.ToString());
                }
            }
        }
        /// <summary>
        /// 根据标头信息生成报表。
        /// </summary>
        /// <returns></returns>
        public DataTable Generate()
        {
            this.ConstructReport();
            this.LoadSeekColumn();
            this.LoadComputedColumn();

            return this.ReportTable;
        }
    }
}
