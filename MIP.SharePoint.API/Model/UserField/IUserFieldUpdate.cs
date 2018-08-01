using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Model.UserField
{
    public interface IUserFieldUpdate
    {
        string InternalFieldName { get; set; }
        string UserName { get; set; }
    }
}
