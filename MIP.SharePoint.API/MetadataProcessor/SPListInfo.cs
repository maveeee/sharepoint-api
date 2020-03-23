using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MIP.SharePoint.API.Helper;
using MIP.SharePoint.API.Model.Attributes;

namespace MIP.SharePoint.API.MetadataProcessor
{
    public class SPListInfo : ISPListInfo
    {
        public Type Type { get; set; }
        public string ListAlias { get; set; }
        public Uri ListUri { get; set; }

        public bool HasAttachments { get; }
        public bool IsDocumentLibrary { get; }

        private readonly Lazy<Func<object, IEnumerable<IDocument>>> attachmentsFunc;
        private readonly Lazy<Func<object, IDocument>> documentFunc;

        public SPListInfo(Type type, Uri uri)
        {
            var spListAttribute = AttributeHelper.GetAttribute<SPListAttribute>(type);

            if (spListAttribute == null)
            {
                throw new ArgumentException($"The type '{type}' does not specify the attribute '{typeof(SPListAttribute)}'!", nameof(type));
            }

            this.ListAlias = spListAttribute.Name;
            this.IsDocumentLibrary = spListAttribute.Type == SPListAttribute.ListType.DocumentLibrary;
            this.HasAttachments = GetAttachmentsProperty(type) != null;

            if (IsDocumentLibrary)
            {
                this.documentFunc = new Lazy<Func<object, IDocument>>(() =>
                {
                    var documentProperty = GetDocumentProperty(this.Type);
                    return obj => (IDocument)documentProperty.GetValue(obj);
                });
            }

            if (HasAttachments)
            {
                this.attachmentsFunc = new Lazy<Func<object, IEnumerable<IDocument>>>(() =>
                {
                    var attachmentsProperty = GetAttachmentsProperty(this.Type);
                    return obj => (IEnumerable<IDocument>)attachmentsProperty.GetValue(obj);
                });
            }
            
            this.Type = type;
            this.ListUri = uri;
        }

        public Func<object, IEnumerable<IDocument>> GetAttachmentsFunction()
        {
            if (!HasAttachments)
            {
                throw new Exception("The current list info does have define attachments.");
            }

            return this.attachmentsFunc?.Value;
        }

        public Func<object, IDocument> GetDocumentFunction()
        {
            if (!IsDocumentLibrary)
            {
                throw new Exception("The current list info does not describe a document library.");
            }

            return this.documentFunc?.Value;
        }

        private static PropertyInfo GetAttachmentsProperty(Type listModelType)
        {
            var attachmentsProperty = listModelType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                .FirstOrDefault(propInfo => typeof(IEnumerable<IDocument>).IsAssignableFrom(propInfo.PropertyType));

            return attachmentsProperty;
        }

        private static PropertyInfo GetDocumentProperty(Type listModelType)
        {
            var documentProperty = listModelType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                .FirstOrDefault(propInfo => propInfo.PropertyType.IsAssignableFrom(typeof(IDocument)));

            return documentProperty;
        }
    }
}