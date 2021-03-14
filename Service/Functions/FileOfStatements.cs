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
    /// 存储Sql语句的文件。
    /// </summary>
    public class FileOfStatements
    {
        public string FilePath { get; set; }

        public FileOfStatements(string filePath)
        {
            this.FilePath = filePath;
        }

        public string Statements
        {
            get
            {
                return FileHelper.ReadText(this.FilePath);
            }
        }

        public virtual DataTable QueryResult()
        {
            return DBHelper4MySQL.Query(this.Statements).MergeAllTables();
        }
    }
}
