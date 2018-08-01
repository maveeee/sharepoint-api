using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Caml
{
    internal class Queries
    {
        internal static CamlQuery GetItems(int rowLimit)
        {
            return new CamlQuery()
            {
                ViewXml = string.Format("<View Scope='RecursiveAll'>" +
                "<RowLimit>{0}</RowLimit>" +
                "<Query>" +
                "<Where>" +
                "</Where>" +
                "</Query>" +
                "</View>", rowLimit)
            };
        }
        internal static CamlQuery GetItems(int rowLimit, int offsetInDays)
        {
            return new CamlQuery()
            {
                ViewXml = string.Format("<View Scope='RecursiveAll'>" +
                "<RowLimit>{0}</RowLimit>" +
                "<Query>" +
                "<Where>" +
                "<Geq>" +
                "<FieldRef Name='Modified' />" +
                "<Value Type='DateTime'>" +
                "<Today OffsetDays='-{1}' />" +
                "</Value></Geq>" +
                "</Where>" +
                "</Query>" +
                "</View>", rowLimit, offsetInDays)
            };
        }
        internal static CamlQuery GetItems(string searchColumn, string searchValue)
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
                "</View>", searchColumn, searchValue)
            };
        }
    }
}
