using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Model.Attachment
{
    public class FileAttachment
    {
        public string Name { get; set; }
        public byte[] File { get; set; }
    }
}
