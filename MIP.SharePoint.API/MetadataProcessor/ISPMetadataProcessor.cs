using System.Collections.Generic;
using MIP.SharePoint.API.Model;

namespace MIP.SharePoint.API.MetadataProcessor
{
    public interface ISPMetadataProcessor
    {
        IEnumerable<IDocument> GetAttachments(object listModel);
        IDocument GetDocument(object listModel);
        MetaData GetMetaData(object listModel);
    }
}