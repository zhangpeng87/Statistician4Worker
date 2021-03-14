using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Functions
{
    /// <summary>
    /// 标段信息。
    /// </summary>
    public class Project
    {
        /// <summary>
        /// 标段id。
        /// </summary>
        public int project_id;
        /// <summary>
        /// 标段名称。
        /// </summary>
        public string project_title;
        /// <summary>
        /// 标段编码。
        /// </summary>
        public string tag_id;
        /// <summary>
        /// 参建单位列表。
        /// </summary>
        public IList<Cooperator> cooperators;
    }
}
