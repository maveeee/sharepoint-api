using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Model.Field
{
    public class FieldUpdate : IFieldUpdate
    {
        public string InternalFieldName { get; set; }
        public object FieldValue { get; set; }
        public Type Type { get; set; }
    }
}
