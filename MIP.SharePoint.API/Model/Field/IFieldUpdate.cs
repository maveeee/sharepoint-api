using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Model.Field
{
    public interface IFieldUpdate
    {
        string InternalFieldName { get; set; }
        object FieldValue { get; set; }
        Type Type { get; set; }
    }
}
