using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Utils
{
    public static class DataSetExtensions
    {
        /// <summary>
        /// 合并此DataSet中的所有Tables（须有共同的列）。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DataTable MergeAllTables(this DataSet source)
        {
            DataTable result = new DataTable();

            for (int i = 0; i < source.Tables.Count; i++)
            {
                result.Merge(source.Tables[i]);
            }

            return result;
        }
    }
}
