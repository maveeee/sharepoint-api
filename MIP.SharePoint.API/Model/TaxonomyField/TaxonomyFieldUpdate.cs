using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Model.TaxonomyField
{
    public class TaxonomyFieldUpdate : ITaxonomyFieldUpdate
    {
        public string InternalFieldName { get; set; }
        public string FieldValue { get; set; }
    }
}
