using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Model.UserField
{
    public class UserFieldUpdate : IUserFieldUpdate
    {
        public string InternalFieldName { get; set; }
        public string UserName { get; set; }
    }
}
