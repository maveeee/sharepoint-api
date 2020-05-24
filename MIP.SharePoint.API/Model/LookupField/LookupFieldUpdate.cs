using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Model.LookupField
{
    public class LookupFieldUpdate : ILookupFieldUpdate
    {
        public string InternalFieldName { get; set; }
        public string ListUrl { get; set; }
        public string ColumnToSearch { get; set; }
        public string SearchText { get; set; }
        public bool UseLookupId { get; set; } = false;
        public int LookupId { get; set; }
        public bool UseMultipleLookupIds { get; set; }
        public List<int> LookupIds { get; set; } = new List<int>();
    }
}
