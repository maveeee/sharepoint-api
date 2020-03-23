using System;

namespace MIP.SharePoint.API.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SPFieldAttribute : Attribute
    {
        public string Name { get; }
        public Type Type { get; }

        public SPFieldAttribute(string name) : this(name, null)
        {
        }

        public SPFieldAttribute(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}