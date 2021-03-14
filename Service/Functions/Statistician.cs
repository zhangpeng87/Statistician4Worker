using Service.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Service.Functions
{
    /// <summary>
    /// 数据统计类。
    /// </summary>
    public class Statistician
    {
        private string _path;

        public Statistician(string path)
        {
            this._path = path;
        }

        public DataTable GetResult()
        {
            var paths = Directory.EnumerateFiles(this._path).OrderBy(e => e);

            TableConnector connector = null;
            foreach (string filePath in paths)
            {
                FileOfStatements file = new FileOfStatements(filePath);

                if (connector == null)
                {
                    connector = TableConnector.NewJoin(() => file.QueryResult() );
                }
                else
                {
                    file = new RecountByGroup(file);
                    connector = connector.LeftJoin(() => file.QueryResult() );
                }
            }

            return connector.GenerateTable();
        }
    }
}
