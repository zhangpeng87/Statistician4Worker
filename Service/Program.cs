using Polly;
using Service.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Service.Utils;
using System.Data.SqlClient;
using System.ServiceProcess;

namespace Service
{
    class Program
    {
        #region Nested classes to support running as service
        public const string ServiceName = "StatisStaffService";

        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = Program.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                Program.Start(args);
            }

            protected override void OnStop()
            {
                Program.Stop();
            }
        }
        #endregion

        private static Timer timer;
        private static readonly int period = 3; // 间隔3分钟

        static Program()
        {
            // 0 表示不向整点表插入数据
            //timer = new Timer(TimerCallback, 0, 1000, 1000 * 60 * period);
        }

        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
                // running as service
                using (var service = new Service())
                    ServiceBase.Run(service);
            else
            {
                // running as console app
                Start(args);

                Console.WriteLine("StatisStaffService 程序正在运行中... ...");
                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);

                Stop();
            }
        }

        private static void Start(string[] args)
        {
            LogUtils.Logger.Info($"StatisStaffService 服务程序开始运行... ...");
            // 0 表示不向整点表插入数据
            timer = new Timer(TimerCallback, 0, 1000, 1000 * 60 * period);
        }

        private static void Stop()
        {
            // onstop code here
        }

        /// <summary>
        /// 校准时间定时器。
        /// </summary>
        /// <param name="timer"></param>
        private static bool CalibrationTimer(Timer timer, DateTime now)
        {
            // 下个整点钟
            string strNext = now.AddHours(1).ToString("yyyy-MM-dd HH:00");
            DateTime next = DateTime.Parse(strNext);

            // 是否需校准
            var diff = next.Subtract(DateTime.Now);
            if (diff < TimeSpan.FromMinutes(period))
                return timer.Change((int)diff.TotalMilliseconds, 1000 * 60 * period);
            else
                return false;
        }

        /// <summary>
        /// 定时任务。
        /// </summary>
        /// <param name="state"></param>
        private static void TimerCallback(object state)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();                                  //  开始监视代码运行时间

            #region 获取完整的统计数据
            // Sql文件目录
            string path = @"D:\StatisStaffService\SqlStatements";
            var s = new Statistician(path);
            DataTable result = null;

            try
            {
                // 查询统计数据
                result = s.GetResult()
                        .AsEnumerable()
                        .OrderBy(r => r.Field<string>(0))
                        .CopyToDataTable();
            }
            catch (Exception e)
            {
                string msg = $"抛出异常：查询统计数据；异常消息：{ e.Message }。";
                LogUtils.Logger.Error(msg);
                Console.WriteLine(msg);
                return;
            }

            // 获取统计时间
            DateTime now = DateTime.Now;
            // 添加时间一列
            var time = new DataColumn("statis_time", typeof(DateTime)) { DefaultValue = Convert.ToDateTime(now.ToString("yyyy-MM-dd HH:mm")) };
            result.Columns.Add(time);
            time.SetOrdinal(0);

            // 校准定时器
            bool changed = CalibrationTimer(timer, now);
            // 是否是整点
            bool whether = (now.Minute == 0);

            try
            {
                // 保存统计结果
                using (var command = new SqlCommand("insert_statis_worker") { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Add(new SqlParameter("@myTable", result));
                    command.Parameters.Add(new SqlParameter("@whetherCopy", whether));
                    DbHelperSQL.Exec(command);
                }
            }
            catch (Exception e)
            {
                string msg = $"抛出异常：保存统计结果；异常消息：{ e.Message }。";
                LogUtils.Logger.Error(msg);
                Console.WriteLine(msg);
            }

            #endregion

            stopwatch.Stop();                                   //  停止监视
            TimeSpan timespan = stopwatch.Elapsed;              //  获取当前实例测量得出的总时间
            double hours = timespan.TotalHours;                 // 总小时
            double minutes = timespan.TotalMinutes;             // 总分钟
            double seconds = timespan.TotalSeconds;             //  总秒数
            double milliseconds = timespan.TotalMilliseconds;   //  总毫秒数

            LogUtils.Logger.Info($"统计完成时间：{ now.ToString("yyyy-MM-dd HH:mm:ss") }；统计耗费时间：{ seconds }秒；统计结果行数：{ result?.Rows.Count ?? 0 }。");
        }
    }
}
