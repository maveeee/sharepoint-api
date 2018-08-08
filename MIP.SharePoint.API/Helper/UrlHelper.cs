using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Helper
{
    public class UrlHelper
    {
        public static bool IsAbsoluteUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri result);
        }
        public static string GetAbsoluteUrl(string contextUrl, string relativeUrl)
        {
            return $"{new Uri(contextUrl).GetLeftPart(UriPartial.Authority)}{relativeUrl}";
        }
    }
}
