using MIP.SharePoint.API.Model.Attributes;

namespace MIP.SharePoint.API.Samples.MetadataProcessor.Model
{
    [SPList("AliasOfTheReferencedList", Type = SPListAttribute.ListType.DocumentLibrary)]
    public class MyOtherListModel
    {
        [SPField("Key")]
        public string Key { get; set; }

        [SPField("InternalFieldNameInTheReferencedList")]
        public int SomeColumn { get; set; }
        
    }
}
