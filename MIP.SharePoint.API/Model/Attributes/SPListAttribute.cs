using System;

namespace MIP.SharePoint.API.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SPListAttribute : Attribute
    {
        public string Name { get; }

        public ListType Type { get; set; } = ListType.List;

        public SPListAttribute(string name)
        {
            Name = name;
        }

        public enum ListType
        {
            List,
            DocumentLibrary
        }
    }
}