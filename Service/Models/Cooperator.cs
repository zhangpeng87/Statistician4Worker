using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Functions
{
    /// <summary>
    /// 参建单位信息及统计。
    /// </summary>
    public class Cooperator
    {
        /// <summary>
        /// 参建单位id。
        /// </summary>
        public int cooperator_id;
        /// <summary>
        /// 所属标段id。
        /// </summary>
        public int project_id;
        /// <summary>
        /// 单位名称。
        /// </summary>
        public string unit_name;
        /// <summary>
        /// 单位类型。
        /// </summary>
        public int unit_type;
        /// <summary>
        /// 班组/部门列表。
        /// </summary>
        public IList<Group> groups;
        /// <summary>
        /// 统计信息。
        /// </summary>
        public Statistics statistics;
    }
}
