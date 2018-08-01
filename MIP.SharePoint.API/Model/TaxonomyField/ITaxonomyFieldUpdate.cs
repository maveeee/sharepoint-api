using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Model.TaxonomyField
{
    public interface ITaxonomyFieldUpdate
    {
        string InternalFieldName { get; set; }
        string FieldValue { get; set; }
    }
}
