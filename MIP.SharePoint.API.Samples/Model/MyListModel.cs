using System.Collections.Generic;
using MIP.SharePoint.API.MetadataProcessor;
using MIP.SharePoint.API.Model.Attributes;

namespace MIP.SharePoint.API.Samples.MetadataProcessor.Model
{
    [SPList("MyList", Type = SPListAttribute.ListType.DocumentLibrary)]
    public class MyListModel
    {
        [SPField("Title")]
        public string Title { get; set; }

        [SPField("SPInternalFieldName")]
        public int SomeColumn { get; set; }

        public string NotMappedProperty { get; set; }

        [SpLookupField("AnotherInternalFieldName", "AliasOfTheReferencedList", "InternalFieldNameInTheReferencedList")]
        public string SomeLookupField { get; set; }

        /// <summary>
        /// A property whose type is assignable to <see cref="IDocument"/> is automatically picked up as the document of the document library
        /// </summary>
        public IDocument Document { get; set; }

        /// <summary>
        /// A property whose type is assignable to <see cref="IEnumerable{IDocument}"/> is automatically picked up as the source of attachments
        /// </summary>
        public IEnumerable<IDocument> Attachments { get; set; }
    }
}
