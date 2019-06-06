using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Helper
{
    public static class HashHelper
    {
        public static string GetHashFromStream(System.IO.Stream stream)
        {
            
            using(var sha1 = SHA1Managed.Create())
            {
                return Convert.ToBase64String(sha1.ComputeHash(stream));
            }
        }
    }
}
