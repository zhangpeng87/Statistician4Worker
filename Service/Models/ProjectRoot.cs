using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Functions
{
    /// <summary>
    /// 项目全场信息。
    /// </summary>
    public class ProjectRoot
    {
        /// <summary>
        /// 统计时间。
        /// </summary>
        public string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        /// <summary>
        ///  项目名称。
        /// </summary>
        public string name = "鄂州花湖机场";
        /// <summary>
        /// 标段列表。
        /// </summary>
        public IList<Project> projects;
        /// <summary>
        /// 统计信息。
        /// </summary>
        public Statistics statistics;
    }
}
