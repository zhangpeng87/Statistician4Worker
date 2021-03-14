using MySql.Data.MySqlClient;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Service.Utils
{
    class DBHelper4MySQL
    {
        private static SshClient client;
        private static ForwardedPortLocal portForwarded;

        static DBHelper4MySQL()
        {
            client = new SshClient("122.189.155.124", 9011, "root", "Admin20181028");
            client.Connect();

            if (client.IsConnected)
            {
                portForwarded = new ForwardedPortLocal(IPAddress.Loopback.ToString(), 3306, "10.0.0.201", 3306);

                client.AddForwardedPort(portForwarded);
                portForwarded.Start();
            }
        }

        public static DataSet Query(string SQLString, string dbName = "zhgd_lw")
        {
            DataSet ds = new DataSet();

            if (client.IsConnected)
            {
                using (MySqlConnection connection = new MySqlConnection($"SERVER={ portForwarded.BoundHost };PORT={ portForwarded.BoundPort };DATABASE={ dbName };UID=zzsa;PASSWORD=NL88tNkfHnE3kFgT"))
                {
                    try
                    {
                        MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                        
                        command.Fill(ds, "ds");
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        connection?.Close();
                    }
                }
            }

            DataSet newSet = new DataSet();
            foreach (DataTable source in ds.Tables)
                newSet.Tables.Add(ChangeDataType(source));

            return newSet;
        }

        /// <summary>
        /// 数据列作数据类型转换。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static DataTable ChangeDataType(DataTable source)
        {
            DataTable dtCloned = source.Clone();

            foreach (DataColumn column in dtCloned.Columns)
            {
                if (column.DataType == typeof(sbyte))
                    column.DataType = typeof(int);
            }

            foreach (DataRow row in source.Rows)
            {
                dtCloned.ImportRow(row);
            }

            return dtCloned;
        }
    }
}
