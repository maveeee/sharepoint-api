using System;

namespace MIP.SharePoint.API.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SpLookupFieldAttribute : SPFieldAttribute
    {
        public string LookupListName { get; }
        public string LookupListFieldName { get; }

        public SpLookupFieldAttribute(string name, string lookupListName, string lookupListFieldName) : base(name, typeof(string))
        {
            this.LookupListName = lookupListName;
            this.LookupListFieldName = lookupListFieldName;
        }
    }
}