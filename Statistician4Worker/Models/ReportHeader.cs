using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Statistician4Worker.Models
{
    public class ReportHeader
    {
        public string HeaderText { get; set; }
        public object State { get; set; }
    }
    
    public class ReportSeekHeader : ReportHeader
    {
        public Func<object, IEnumerable> ValueHandler { get; set; }
    }
    
    public class ReportComputedHeader : ReportHeader
    {
        public Func<object, IDictionary> ValueHandler { get; set; }
        public string Sort { get; set; }
    }

    public class ReportHeaderCollection
    {
        public ReportSeekHeader SeekHeader { get; set; }
        public IList<ReportComputedHeader> ComputedHeaders { get; set; }
        public IEnumerable<ReportHeader> AllReportHeaders
        {
            get
            {
                var list = new ReportHeader[ComputedHeaders.Count + 1];
                list[0] = this.SeekHeader;
                for (int i = 0; i < ComputedHeaders.Count; i++)
                    list[i + 1] = SortedComputedHeaders.ElementAt(i);

                return list;
            }
        }
        public ReportHeaderCollection(ReportSeekHeader seekHeader)
        {
            this.SeekHeader = seekHeader;
            ComputedHeaders = new List<ReportComputedHeader>();
        }

        public void AddComputedHeader(ReportComputedHeader header)
        {
            this.ComputedHeaders.Add(header);
        }
        private IEnumerable<ReportComputedHeader> SortedComputedHeaders
        {
            get
            {
                return this.ComputedHeaders.OrderBy(e => e.Sort);
            }
        }
    }
}
