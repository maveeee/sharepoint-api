﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using MIP.SharePoint.API.Helper;
using MIP.SharePoint.API.Model;
using MIP.SharePoint.API.Model.Attributes;
using MIP.SharePoint.API.Model.Field;
using MIP.SharePoint.API.Model.LookupField;
using MIP.SharePoint.API.Model.TaxonomyField;
using MIP.SharePoint.API.Model.UserField;

namespace MIP.SharePoint.API.MetadataProcessor
{
    public class SPMetadataProcessor : ISPMetadataProcessor
    {
        private readonly ISPListInfoLookup listInfoLookup;

        public SPMetadataProcessor()
        {
            this.listInfoLookup = new SPListInfoLookup();
        }

        public SPMetadataProcessor(ISPListInfoLookup listInfoLookup)
        {
            this.listInfoLookup = listInfoLookup;
        }

        public IEnumerable<IDocument> GetAttachments(object listModel)
        {
            if (listModel == null)
            {
                throw new ArgumentNullException(nameof(listModel));
            }

            var listInfo = this.listInfoLookup.GetByType(listModel.GetType());
            var attachmentsFunction = listInfo.GetAttachmentsFunction();

            if (attachmentsFunction == null)
            {
                return new IDocument[0];
            }

            return attachmentsFunction(listModel);
        }

        public IDocument GetDocument(object listModel)
        {
            if (listModel == null) throw new ArgumentNullException(nameof(listModel));

            var listInfo = this.listInfoLookup.GetByType(listModel.GetType());
            var documentFunction = listInfo.GetDocumentFunction();

            if (documentFunction == null) throw new Exception($"The given object (Type: {listModel.GetType()}) does not specify a function to retrieve the document data (Type: {typeof(IDocument)})");

            return documentFunction(listModel);
        }

        public MetaData GetMetaData(object listModel)
        {
            var metadata = new MetaData();

            var fieldUpdates = listModel.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                .Select(propertyInfo =>
                {
                    var attribute = AttributeHelper.GetAttribute<SPFieldAttribute>(propertyInfo);
                    return (propertyInfo, attribute);
                })
                .Where(tuple => tuple.attribute != null)
                .Select(tuple => ToFieldUpdateType(listModel, tuple.propertyInfo, tuple.attribute))
                .Where(fieldUpate => fieldUpate != null)
                .GroupBy(x => x.GetType())
                .ToDictionary(group => group.Key, group => group.ToList());

            if (fieldUpdates.ContainsKey(typeof(FieldUpdate)))
            {
                metadata.UpdateValues = fieldUpdates[typeof(FieldUpdate)].Cast<IFieldUpdate>().ToList();
            }

            if (fieldUpdates.ContainsKey(typeof(LookupFieldUpdate)))
            {
                metadata.LookupFields = fieldUpdates[typeof(LookupFieldUpdate)].Cast<ILookupFieldUpdate>().ToList();
            }

            if (fieldUpdates.ContainsKey(typeof(UserFieldUpdate)))
            {
                metadata.UserFields = fieldUpdates[typeof(UserFieldUpdate)].Cast<IUserFieldUpdate>().ToList();
            }

            if (fieldUpdates.ContainsKey(typeof(TaxonomyFieldUpdate)))
            {
                metadata.TaxonomyFields = fieldUpdates[typeof(TaxonomyFieldUpdate)].Cast<ITaxonomyFieldUpdate>().ToList();
            }

            return metadata;
        }

        // This method returns object as IFieldUpdate and ILookupFieldUpdate share no common base type
        private object ToFieldUpdateType(object listModel, PropertyInfo propertyInfo, SPFieldAttribute attribute)
        {
            return attribute switch
            {
                SpLookupFieldAttribute lookupFieldAttribute => ToLookupFieldUpdate(listModel, propertyInfo, lookupFieldAttribute),
                SPUserFieldAttribute userFieldAttribute => ToUserFieldUpdate(listModel, propertyInfo, userFieldAttribute),
                SPTaxonomyFieldAttribute taxonomyFieldAttribute => ToTaxonomyFieldUpdate(listModel, propertyInfo, taxonomyFieldAttribute),
                SPFieldAttribute fieldAttribute => ToFieldUpdate(listModel, propertyInfo, fieldAttribute),
                _ => throw new ArgumentOutOfRangeException($"Unexpected type '{attribute.GetType()}'", nameof(attribute)),
            };
        }

        private static FieldUpdate ToFieldUpdate(object listModel, PropertyInfo propertyInfo, SPFieldAttribute attribute)
        {
            var fieldName = attribute.Name;
            var fieldType = attribute.Type ?? propertyInfo.PropertyType;
            var value = propertyInfo.GetValue(listModel);

            var fieldUpdate = new FieldUpdate()
            {
                InternalFieldName = fieldName,
                Type = fieldType,
                FieldValue = value,
            };

            return fieldUpdate;
        }

        private LookupFieldUpdate ToLookupFieldUpdate(object listModel, PropertyInfo propertyInfo, SpLookupFieldAttribute attribute)
        {
            var fieldName = attribute.Name;
            var value = (string)Convert.ChangeType(propertyInfo.GetValue(listModel), typeof(string)); // TODO check for a better way to convert to string
            var lookupListName = attribute.LookupListName;
            var lookupListFieldName = attribute.LookupListFieldName;

            // If no value is set, do not create a LookupFieldUpdate object
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var listUrl = this.listInfoLookup.GetByAlias(lookupListName).ListUri?.ToString();

            var fieldUpdate = new LookupFieldUpdate()
            {
                InternalFieldName = fieldName,
                SearchText = value,
                ListUrl = listUrl,
                ColumnToSearch = lookupListFieldName,
            };

            return fieldUpdate;
        }

        private UserFieldUpdate ToUserFieldUpdate(object listModel, PropertyInfo propertyInfo, SPUserFieldAttribute attribute)
        {
            return ToStringFieldUpdate(listModel, propertyInfo, attribute, (name, value) => new UserFieldUpdate()
            {
                InternalFieldName = name,
                UserName = value
            });
        }

        private TaxonomyFieldUpdate ToTaxonomyFieldUpdate(object listModel, PropertyInfo propertyInfo, SPTaxonomyFieldAttribute attribute)
        {
            return ToStringFieldUpdate(listModel, propertyInfo, attribute, (name, value) => new TaxonomyFieldUpdate()
            {
                InternalFieldName = name,
                FieldValue = value
            });
        }


        private T ToStringFieldUpdate<T>(object listModel, PropertyInfo propertyInfo, SPFieldAttribute attribute, Func<string, string, T> fieldUpdateFactory) where T : class
        {
            if (propertyInfo.PropertyType != typeof(string))
            {
                throw new Exception($"Field property type needs to be of type string! (actual: {propertyInfo.PropertyType})");
            }

            var value = (string)propertyInfo.GetValue(listModel);

            // If no value is set, do not create a UserFieldUpdate object
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var fieldUpdate = fieldUpdateFactory(attribute.Name, value);

            return fieldUpdate;
        }
    }
}
