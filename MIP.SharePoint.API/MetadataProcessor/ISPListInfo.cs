using System;
using System.Collections.Generic;

namespace MIP.SharePoint.API.MetadataProcessor
{
    public interface ISPListInfo
    {
        Type Type { get; }
        string ListAlias { get; }
        Uri ListUri { get; }

        bool HasAttachments { get; }
        bool IsDocumentLibrary { get; }

        Func<object, IEnumerable<IDocument>> GetAttachmentsFunction();
        Func<object, IDocument> GetDocumentFunction();
    }
}