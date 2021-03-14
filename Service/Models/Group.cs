using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Functions
{
    /// <summary>
    /// 班组部门信息及统计。
    /// </summary>
    public class Group
    {
        /// <summary>
        /// 班组部门id。
        /// </summary>
        public int group_id;
        /// <summary>
        /// 所属单位id。
        /// </summary>
        public int cooperator_id;
        /// <summary>
        /// 班组名称。
        /// </summary>
        public string name;
        /// <summary>
        /// 统计信息。
        /// </summary>
        public Statistics statistics;
    }
}
