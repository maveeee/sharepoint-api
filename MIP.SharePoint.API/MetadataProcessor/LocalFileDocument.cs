using System;
using System.IO;

namespace MIP.SharePoint.API.MetadataProcessor
{
    public class LocalFileDocument : IDocument
    {
        public string FileName { get; set; }

        public string FullPath { get; set; }

        public Stream GetContent()
        {
            if (string.IsNullOrEmpty(this.FullPath))
            {
                throw new InvalidOperationException("Cannot get attachment content stream if FullPath is not set!");
            }

            return new FileStream(FullPath, FileMode.Open);
        }
    }
}