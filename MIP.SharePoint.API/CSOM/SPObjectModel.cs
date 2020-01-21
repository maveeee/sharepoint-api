using Microsoft.SharePoint.Client;
using MIP.SharePoint.API.Model;
using MIP.SharePoint.API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIP.SharePoint.API.Extensions;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client.DocumentSet;
using MIP.SharePoint.API.Helper;

namespace MIP.SharePoint.API.CSOM
{
    public class SPObjectModel
    {
        private const int SP_QUERY_ROW_LIMIT = 4999;
        public byte[] DownloadFile(ClientContext ctx, string relativeUrl, string listName, string fileName)
        {
            if (ctx.HasPendingRequest)
                ctx.ExecuteQuery();

            return StreamUtils.GetStreamAsByteArray(File.OpenBinaryDirect(ctx, $"{relativeUrl}/{listName}/{fileName}").Stream);
        }
        public string UploadFile(ClientContext ctx, string listUrl, string fileName, byte[] file, string relativeFolderUrl = null)
        {
            var list = GetListByUrl(ctx, listUrl);
            ctx.Load(list.RootFolder);
            ctx.ExecuteQuery();

            var uploadFolderUrl = list.RootFolder.ServerRelativeUrl;
            if (!String.IsNullOrEmpty(relativeFolderUrl))
                uploadFolderUrl = relativeFolderUrl;

            var fileUrl = String.Format("{0}/{1}", uploadFolderUrl, fileName);

            using (var fileStream = new System.IO.MemoryStream(file))
            {
                Microsoft.SharePoint.Client.File.SaveBinaryDirect(ctx, fileUrl, fileStream, true);
            }
            return fileUrl;
        }
        public void SaveAttachment(ClientContext ctx, ListItem item, string fileName, byte[] attachment)
        {
            var attachmentInfo = new AttachmentCreationInformation
            {
                FileName = fileName
            };
            using (var fileStream = new System.IO.MemoryStream(attachment))
            {
                attachmentInfo.ContentStream = fileStream;
                item.AttachmentFiles.Add(attachmentInfo);
                ctx.ExecuteQuery();
            }
        }
        public string GetRootFolderName(ClientContext ctx, List list)
        {
            var rootFolder = list.RootFolder;
            ctx.Load(rootFolder);
            ctx.ExecuteQuery();

            return list.RootFolder.Name;
        }
        private void LoadDefaultListProperties(ClientContext ctx, List list)
        {
            ctx.Load(list, x => x.Id);
            ctx.Load(list, x => x.Title);
            ctx.Load(list, x => x.Fields);
            ctx.Load(list, x => x.RootFolder);
            ctx.Load(list, x => x.RootFolder.Name);

            ctx.ExecuteQuery();
        }
        public List GetListById(ClientContext ctx, Guid listId)
        {
            var list = ctx.Web.Lists.GetById(listId);
            LoadDefaultListProperties(ctx, list);
            return list;
        }
        public List GetListByUrl(ClientContext ctx, string listUrl)
        {
            var list = ctx.Web.GetList(listUrl);
            LoadDefaultListProperties(ctx, list);
            return list;
        }
        private ListItemCollection ListItemQuery(ClientContext ctx, List list, CamlQuery query)
        {
            var items = ctx.Web.Lists.GetById(list.Id).GetItems(query);
            ctx.Load(items);

            ctx.ExecuteQuery();

            return items;

        }
        private List<ListItem> GetListItemsInternal(ClientContext ctx, List list, CamlQuery camlQuery)
        {
            var items = new List<ListItem>();

            var query = new CamlQuery();
            if(query != null)
            {
                query = camlQuery;
            }


            ListItemCollection listItemCollection = null;
            do
            {
                listItemCollection = ListItemQuery(ctx, list, query);

                if (listItemCollection.ListItemCollectionPosition != null)
                    query.ListItemCollectionPosition = listItemCollection.ListItemCollectionPosition;

                if (listItemCollection != null)
                    items.AddRange(listItemCollection);

            }
            while (listItemCollection.ListItemCollectionPosition != null);

            return items;
        }
        public List<ListItem> GetListItems(ClientContext ctx, List list, CamlQuery camlQuery)
        {
            return GetListItemsInternal(ctx, list, camlQuery);
        }
        public List<ListItem> GetListItems(ClientContext ctx, List list, int modifyOffsetInDays = 0)
        {
            var items = new List<ListItem>();

            var camlQuery = new CamlQuery();
            if (modifyOffsetInDays != 0)
            {
                camlQuery = Caml.Queries.GetItems(SP_QUERY_ROW_LIMIT, modifyOffsetInDays);
                return GetListItemsInternal(ctx, list, camlQuery);
            }
            else
            {
                camlQuery = Caml.Queries.GetItems(SP_QUERY_ROW_LIMIT);
                return GetListItemsInternal(ctx, list, camlQuery);
            }

        }
        public void DeleteListItems(ClientContext ctx, List<ListItem> listItemsToDelete)
        {
            foreach(var listItem in listItemsToDelete)
            {
                listItem.DeleteObject();
                ctx.ExecuteQueryWithIncrementalRetry();
            }
            
        }
        public int GetLookupId(ClientContext ctx, string listUrl, string searchColumn, string searchText)
        {
            var list = GetListByUrl(ctx, listUrl);

            var camlQuery = Caml.Queries.GetItems(searchColumn, searchText);
            var collection = ListItemQuery(ctx, list, camlQuery);
            
            return collection.First().Id;
        }
        public string GetTermId(ClientContext ctx, string term, Guid termSetId)
        {
            var taxonomySession = TaxonomySession.GetTaxonomySession(ctx);
            ctx.Load(taxonomySession);

            var termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
            ctx.Load(termStore);

            var termSet = termStore.GetTermSet(termSetId);
            ctx.Load(termSet);

            var termMatches = termSet.GetTerms(new LabelMatchInformation(ctx)
            {
                TrimUnavailable = true,
                TermLabel = term,
            });
            ctx.Load(termMatches);

            ctx.ExecuteQuery();

            //TODO: Handle multiple matches!
            if(termMatches.Count() > 0)
            {
                return termMatches.First().Id.ToString();
            }
            return string.Empty;

        }
        private void SetMetaData(ClientContext ctx, List list, ListItem listItem, MetaData metaData)
        {
            foreach (var updateValue in metaData.UpdateValues)
            {
                if (updateValue.FieldValue != null)
                {
                    dynamic value = Convert.ChangeType(updateValue.FieldValue, updateValue.Type);
                    listItem[updateValue.InternalFieldName] = value;
                }
                else
                {
                    listItem[updateValue.InternalFieldName] = null;
                }
            }
            listItem.Update(); //make sure the item gets updated at the next ExecuteQuery call or else the state of the item crashes

            foreach (var userField in metaData.UserFields)
            {
                var user = ctx.Site.RootWeb.EnsureUser(userField.UserName);
                ctx.Load(user, x => x.Id);
                ctx.ExecuteQueryWithIncrementalRetry();
                var userValue = new FieldUserValue()
                {
                    LookupId = user.Id
                };
                listItem[userField.InternalFieldName] = userValue;
            }

            foreach (var lookupField in metaData.LookupFields)
            {
                listItem[lookupField.InternalFieldName] = new FieldLookupValue()
                {
                    LookupId = GetLookupId(ctx, lookupField.ListUrl, lookupField.ColumnToSearch, lookupField.SearchText),
                };
            }

            foreach(var taxonomyInformation in metaData.TaxonomyFields)
            {
                var field = list.Fields.GetByInternalNameOrTitle(taxonomyInformation.InternalFieldName);

                var txField = ctx.CastTo<TaxonomyField>(field);
                ctx.Load(txField);
                ctx.ExecuteQuery();
                string termId = GetTermId(ctx, taxonomyInformation.FieldValue, txField.TermSetId);

                if(!string.IsNullOrEmpty(termId))
                {
                    if(txField.AllowMultipleValues)
                    {
                        string termValueString = string.Empty;

                        var termValues = listItem[taxonomyInformation.InternalFieldName] as TaxonomyFieldValueCollection;
                        foreach(var taxonomyFieldValue in termValues)
                        {
                            termValueString += $"{taxonomyFieldValue.WssId};#{taxonomyFieldValue.Label}|{taxonomyFieldValue.TermGuid};#"; 
                        }
                        termValueString += $"-1;#{taxonomyInformation.FieldValue}|{termId}";

                        termValues = new TaxonomyFieldValueCollection(ctx, termValueString, txField);
                        txField.SetFieldValueByValueCollection(listItem, termValues);
                    }
                    else
                    {
                        var termValue = new TaxonomyFieldValue
                        {
                            Label = taxonomyInformation.FieldValue,
                            TermGuid = termId,
                            WssId = -1
                        };
                        txField.SetFieldValueByValue(listItem, termValue);
                    }
                }
            }

            listItem.Update();
            ctx.ExecuteQueryWithIncrementalRetry();
        }
        public ListItem CreateItem(ClientContext ctx, List list, MetaData metaData, string folderPath = null)
        {
            return CreateItemInternal(ctx, list, metaData, folderPath);
        }
        public ListItem CreateItem(ClientContext ctx, string listUrl, MetaData metaData, string folderPath = null)
        {
            var list = GetListByUrl(ctx, listUrl);
            ctx.ExecuteQueryWithIncrementalRetry();

            return CreateItemInternal(ctx, list, metaData, folderPath);
        }
        private ListItem CreateItemInternal(ClientContext ctx, List list, MetaData metaData, string folderPath = null)
        {
            var listItemInfo = new ListItemCreationInformation();

            if (!String.IsNullOrEmpty(folderPath))
                listItemInfo.FolderUrl = folderPath;

            var listItem = list.AddItem(listItemInfo);

            this.SetMetaData(ctx, list, listItem, metaData);

            return listItem;
        }
        public void SetMetaData(ClientContext ctx, string listUrl, ListItem listItem, MetaData metaData)
        {
            var list = GetListByUrl(ctx, listUrl);
            ctx.Load(list, x => x.EnableVersioning);

            this.SetMetaData(ctx, list, listItem, metaData);

        }

        public void SetMetaData(ClientContext ctx, string listUrl, string fileUrl, MetaData metaData)
        {
            if(Helper.UrlHelper.IsAbsoluteUrl(fileUrl))
            {
                fileUrl = new Uri(fileUrl).AbsolutePath;
            }
            var uploadedFile = ctx.Web.GetFileByServerRelativeUrl(fileUrl);

            ctx.Load(uploadedFile);
            var list = GetListByUrl(ctx, listUrl);
            ctx.Load(list, x => x.EnableVersioning);

            ctx.ExecuteQuery();

            if (list.EnableVersioning)
            {
                ctx.Load(uploadedFile, x => x.CheckOutType);
                ctx.ExecuteQuery();

                if (uploadedFile.CheckOutType == CheckOutType.None)
                    uploadedFile.CheckOut();

                ctx.ExecuteQuery();
            }

            var listItem = uploadedFile.ListItemAllFields;

            this.SetMetaData(ctx, list, listItem, metaData);

            if (list.EnableVersioning)
                uploadedFile.CheckIn(string.Empty, CheckinType.MajorCheckIn);

            ctx.ExecuteQuery();
        }
        public string CreateFolder(ClientContext ctx, List list, string folderTitle, bool enableFolderCreation = false, string folderUrl = "")
        {
            ctx.Load(list, l => l.EnableFolderCreation);
            ctx.ExecuteQuery();

            if (!list.EnableFolderCreation && enableFolderCreation)
            {
                list.EnableFolderCreation = enableFolderCreation;
                list.Update();
                ctx.ExecuteQuery();
            }
            else if (!list.EnableFolderCreation)
            {
                throw new Exception("Can not create Folders on List because EnableFolderCreation Property is set to false");
            }

            var folderItem = list.AddItem(new ListItemCreationInformation()
            {
                FolderUrl = folderUrl,
                LeafName = folderTitle,
                UnderlyingObjectType = FileSystemObjectType.Folder,
            });

            folderItem.Update();
            ctx.Load(folderItem, folder => folder.Folder.ServerRelativeUrl);
            ctx.ExecuteQuery();

            return UrlHelper.GetAbsoluteUrl(ctx.Url, folderItem.Folder.ServerRelativeUrl);

        }
        private const string DOCUMENT_SET_CONTENT_TYPE_START_ID = "0x0120D520";
        private bool CanAutoDetectDocumentSetContentType(ContentTypeCollection contentTypes)
        {
            var count = contentTypes.Count(x => x.Id.StringValue.StartsWith(DOCUMENT_SET_CONTENT_TYPE_START_ID));
            if (contentTypes.Count(x => x.Id.StringValue.StartsWith(DOCUMENT_SET_CONTENT_TYPE_START_ID)) == 1)
            {
                return true;
            }
            return false;
        }
        public string CreateDocumentSet(ClientContext ctx, List list, string title, bool autoDetectDocumentSetContentType, string contentTypeId = "")
        {
            var rootFolder = list.RootFolder;
            ContentType documentSetContentType = null;

            if (autoDetectDocumentSetContentType)
            {
                var contentTypes = list.ContentTypes;
                ctx.Load(contentTypes);
                ctx.ExecuteQuery();

                if (!CanAutoDetectDocumentSetContentType(contentTypes))
                {
                    throw new Exception("Could not detect a unique Document Set Content Type. Try to use the Content Type Id Param instead");
                }
                documentSetContentType = contentTypes.Single(x => x.Id.StringValue.StartsWith(DOCUMENT_SET_CONTENT_TYPE_START_ID));
            }
            else
            {
                documentSetContentType = list.ContentTypes.GetById(contentTypeId);
                ctx.Load(documentSetContentType);
                ctx.ExecuteQuery();
            }

            var result = DocumentSet.Create(ctx, rootFolder, title, documentSetContentType.Id);
            ctx.ExecuteQuery();

            if (!String.IsNullOrEmpty(result.Value))
                return result.Value;

            return null;
        }
        public ListItem CopyDocument(ClientContext ctx, ClientContext targetCtx, ListItem sourceItem, string listName, string newFileName, bool overwrite = false, MetaData metaData = null)
        {
            return MoveDocument(ctx, targetCtx, sourceItem, listName, newFileName, overwrite, metaData, false);
        }
        public ListItem MoveDocument(ClientContext ctx, ClientContext targetCtx, ListItem sourceItem, string listName, string newFileName, bool overwrite = false, MetaData metaData = null)
        {
            return MoveDocument(ctx, targetCtx, sourceItem, listName, newFileName, overwrite, metaData, true);
        }
        private ListItem MoveDocument(ClientContext ctx, ClientContext targetCtx, ListItem sourceItem, string listName, string newFileName, bool overwrite = false, MetaData metaData = null, bool deleteSourceFile = false)
        {
            ListItem item = null;

            ctx.Load(sourceItem);
            ctx.ExecuteQueryWithIncrementalRetry();

            if(sourceItem.FileSystemObjectType == FileSystemObjectType.File)
            {
                ctx.Load(sourceItem.File);
                ctx.ExecuteQuery();

                listName = listName.TrimStart('/');

                var fileUrl = System.IO.Path.Combine(listName, newFileName);

                var fileInformation = File.OpenBinaryDirect(ctx, sourceItem.File.ServerRelativeUrl);

                var targetWeb = targetCtx.Web;
                targetCtx.Load(targetWeb);
                targetCtx.ExecuteQuery();

                var webServerRelativeUrl = targetWeb.ServerRelativeUrl;
                if (!webServerRelativeUrl.EndsWith(@"/"))
                    webServerRelativeUrl += @"/";

                var relativeFileUrl = webServerRelativeUrl + fileUrl;                    

                File.SaveBinaryDirect(targetCtx, relativeFileUrl, fileInformation.Stream, overwrite);


                if (metaData != null)
                {
                    var movedFile = targetCtx.Web.GetFileByServerRelativeUrl(relativeFileUrl);
                    targetCtx.Load(movedFile);
                    targetCtx.ExecuteQuery();

                    item = movedFile.ListItemAllFields;

                    targetCtx.Load(item);
                    targetCtx.ExecuteQuery();

                    

                    this.SetMetaData(targetCtx, this.GetListByUrl(targetCtx, webServerRelativeUrl + listName), item, metaData);
                }

                if(deleteSourceFile)
                {
                    sourceItem.DeleteObject();
                    ctx.ExecuteQuery();
                }

            }
            else
            {
                throw new Exception("Can't move an Item, unless it's a File");
            }

            return item;
        }

        private string GetFileHash(ClientContext ctx, ListItem item)
        {
            ctx.Load(item);
            ctx.ExecuteQueryWithIncrementalRetry();

            if(item.FileSystemObjectType == FileSystemObjectType.File)
            {
                ctx.Load(item.File);
                ctx.ExecuteQueryWithIncrementalRetry();

                var fileInfo = File.OpenBinaryDirect(ctx, item.File.ServerRelativeUrl);

                return HashHelper.GetHashFromStream(fileInfo.Stream);
            }
            else
            {
                throw new Exception("The List Item must be from FileSystemObjectType File");
            }
        }

        public bool CompareFiles(ClientContext ctx1, ClientContext ctx2, ListItem item1, ListItem item2)
        {
            if (GetFileHash(ctx1, item1) == GetFileHash(ctx2, item2))
                return true;
            
            return false;
        }
    }
}

