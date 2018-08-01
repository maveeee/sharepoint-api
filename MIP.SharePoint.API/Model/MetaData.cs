using MIP.SharePoint.API.Model.Field;
using MIP.SharePoint.API.Model.LookupField;
using MIP.SharePoint.API.Model.TaxonomyField;
using MIP.SharePoint.API.Model.UserField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIP.SharePoint.API.Model
{
    public interface IMetaData
    {

    }
    public class MetaData : IMetaData
    {
        public List<IFieldUpdate> UpdateValues = new List<IFieldUpdate>();
        public List<IUserFieldUpdate> UserFields = new List<IUserFieldUpdate>();
        public List<ILookupFieldUpdate> LookupFields = new List<ILookupFieldUpdate>();
        public List<ITaxonomyFieldUpdate> TaxonomyFields = new List<ITaxonomyFieldUpdate>();
        
    }
}
