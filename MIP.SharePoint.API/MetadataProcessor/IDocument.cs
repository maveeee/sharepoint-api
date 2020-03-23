using System.IO;

namespace MIP.SharePoint.API.MetadataProcessor
{

    public interface IDocument
    {
        string FileName { get; set; }

        /// <summary>
        /// Attention: User needs to close the stream
        /// </summary>
        /// <returns></returns>
        Stream GetContent();
    }
}