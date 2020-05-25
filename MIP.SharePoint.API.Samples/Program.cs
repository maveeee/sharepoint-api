using System;
using System.IO;
using MIP.SharePoint.API.MetadataProcessor;
using MIP.SharePoint.API.Samples.MetadataProcessor.Model;

namespace MIP.SharePoint.API.Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // populate list urls for model classes
            var splistInfoLookup = new SPListInfoLookup
            {
                { typeof(MyListModel), new Uri("http://server.com/mylist") },
                { typeof(MyOtherListModel), new Uri("http://server.com/myotherlist") }
            };


            var metadataProcessor = new SPMetadataProcessor(splistInfoLookup);

            var model = new MyListModel
            {
                Title = "My Title",
                SomeColumn = 6,
                SomeLookupField = "lookup-value",
                TaxonomyColumn = "I have no idea what this is",
                UserColumn = "user@example.com",
                Document = new DummyDocument
                {
                    FileName = Guid.NewGuid().ToString("N")
                },
                Attachments = new IDocument[]
                {
                    new DummyDocument
                    {
                        FileName = Guid.NewGuid().ToString("N")
                    },
                    new DummyDocument
                    {
                        FileName = Guid.NewGuid().ToString("N")
                    },
                }
            };

            // get metadata from object
            var metaData = metadataProcessor.GetMetaData(model); 

            // use metadata


            // ???

            // Profit
        }


        public class DummyDocument : IDocument
        {
            public string FileName { get; set; }
            public Stream GetContent()
            {
                return new MemoryStream(new byte[0]);
            }
        }
    }
}
