using System;

namespace MIP.SharePoint.API.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SPUserFieldAttribute : SPFieldAttribute
    {
        public SPUserFieldAttribute(string name) : base(name, typeof(string))
        {
        }
    }
}