using System;

namespace MIP.SharePoint.API.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SPTaxonomyFieldAttribute : SPFieldAttribute
    {
        public SPTaxonomyFieldAttribute(string name) : base(name, typeof(string))
        {
        }
    }
}