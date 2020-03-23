using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Caml
{
    public class CommonQueries
    {
        public static CamlQuery GetItemsByColumnSearch(string column, string value)
        {
            return new CamlQuery()
            {
                ViewXml = string.Format("<View Scope='RecursiveAll'>" +
               "<RowLimit></RowLimit>" +
               "<Query>" +
               "<Where>" +
               "<Eq>" +
               "<FieldRef Name='{0}' />" +
               "<Value Type='Text'>{1}" +
               "</Value>" +
               "</Eq>" +
               "</Where>" +
               "</Query>" +
               "</View>", column, value)
            };
        }
        public static CamlQuery GetItemsByBooleanFlag(string column, int trueFalse)
        {
            return new CamlQuery()
            {
                ViewXml = string.Format("<View Scope='RecursiveAll'>" +
                "<RowLimit></RowLimit>" +
                "<Query>" +
                "<Where>" +
                "<Eq>" +
                "<FieldRef Name='{0}' />" +
                "<Value Type='Integer'>{1}" +
                "</Value>" +
                "</Eq>" +
                "</Where>" +
                "</Query>" +
                "</View>", column, trueFalse)
            };
        }
    }
}
