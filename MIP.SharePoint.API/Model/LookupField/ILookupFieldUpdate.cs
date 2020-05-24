using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Model.LookupField
{
    public interface ILookupFieldUpdate
    {
        string InternalFieldName { get; set; }
        string ListUrl { get; set; }
        string ColumnToSearch { get; set; }
        string SearchText { get; set; }
        bool UseLookupId { get; set; }
        int LookupId { get; set; }
        bool UseMultipleLookupIds { get; set; }
        List<int> LookupIds { get; set; }
    }
}
