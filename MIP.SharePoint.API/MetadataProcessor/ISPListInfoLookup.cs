using System;
using System.Collections.Generic;

namespace MIP.SharePoint.API.MetadataProcessor
{
    public interface ISPListInfoLookup : ISet<ISPListInfo>
    {
        void Add(Type type, Uri listUri);
        ISPListInfo GetByType(Type type);
        ISPListInfo GetByType<TListEntity>() where TListEntity: class;
        ISPListInfo GetByAlias(string alias);
    }
}
