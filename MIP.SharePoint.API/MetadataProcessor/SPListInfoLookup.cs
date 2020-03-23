using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MIP.SharePoint.API.MetadataProcessor
{
    public class SPListInfoLookup : ISPListInfoLookup
    {
        private readonly HashSet<ISPListInfo> listInfoSet;

        public SPListInfoLookup(ISet<ISPListInfo> listInfoSet)
        {
            this.listInfoSet = new HashSet<ISPListInfo>(listInfoSet);
        }

        public SPListInfoLookup()
        {
            this.listInfoSet = new HashSet<ISPListInfo>();
        }

        IEnumerator<ISPListInfo> IEnumerable<ISPListInfo>.GetEnumerator()
        {
            return listInfoSet.GetEnumerator();
        }

        public IEnumerator<ISPListInfo> GetEnumerator()
        {
            return listInfoSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ISPListInfo GetByType(Type type)
        {
            var listInfo = TryGetByType(type);

            if (listInfo == null)
            {
                throw new KeyNotFoundException($"Cannot find list info for type '{type}'!");
            }

            return listInfo;
        }

        public ISPListInfo GetByType<TListEntity>() where TListEntity : class
        {
            return this.GetByType(typeof(TListEntity));
        }

        public ISPListInfo TryGetByType(Type type)
        {
            return this.listInfoSet.FirstOrDefault(x => x.Type == type);
        }

        public ISPListInfo GetByAlias(string alias)
        {
            var listInfo = TryGetByAlias(alias);

            if (listInfo == null)
            {
                throw new KeyNotFoundException($"Cannot find list info for alias '{alias}'!");
            }

            return listInfo;
        }

        private ISPListInfo TryGetByAlias(string alias)
        {
            var listInfo = this.listInfoSet.FirstOrDefault(x =>
                string.Equals(x.ListAlias, alias, StringComparison.InvariantCultureIgnoreCase));
            
            return listInfo;
        }

        void ICollection<ISPListInfo>.Add(ISPListInfo item)
        {
            listInfoSet.Add(item);
        }

        public void UnionWith(IEnumerable<ISPListInfo> other)
        {
            listInfoSet.UnionWith(other);
        }

        public void IntersectWith(IEnumerable<ISPListInfo> other)
        {
            listInfoSet.IntersectWith(other);
        }

        public void ExceptWith(IEnumerable<ISPListInfo> other)
        {
            listInfoSet.ExceptWith(other);
        }

        public void SymmetricExceptWith(IEnumerable<ISPListInfo> other)
        {
            listInfoSet.SymmetricExceptWith(other);
        }

        public bool IsSubsetOf(IEnumerable<ISPListInfo> other)
        {
            return listInfoSet.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<ISPListInfo> other)
        {
            return listInfoSet.IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<ISPListInfo> other)
        {
            return listInfoSet.IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<ISPListInfo> other)
        {
            return listInfoSet.IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<ISPListInfo> other)
        {
            return listInfoSet.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<ISPListInfo> other)
        {
            return listInfoSet.SetEquals(other);
        }
        public void Add(Type type, Uri listUri)
        {
            ISet<ISPListInfo> thisAsSet = this;
            thisAsSet.Add(new SPListInfo(type, listUri));
        }

        bool ISet<ISPListInfo>.Add(ISPListInfo item)
        {
            if(item == null) throw new ArgumentNullException(nameof(item));
            if(item.ListAlias == null) throw new ArgumentException($"{nameof(ISPListInfo.ListAlias)} property must not be null!", nameof(item));
            if (item.ListUri == null) throw new ArgumentException($"{nameof(ISPListInfo.ListUri)}  property must not be null!", nameof(item));
            if (item.Type == null) throw new ArgumentException($"{nameof(ISPListInfo.Type)}  property must not be null!", nameof(item));

            var listInfo = TryGetByAlias(item.ListAlias);
            if (listInfo != null)
            {
                throw new ArgumentException($"An item with alias '{item.ListAlias}' is already contained in the set!");
            }
            listInfo = TryGetByType(item.Type);
            if (listInfo != null)
            {
                throw new ArgumentException($"An item with type '{item.Type}' is already contained in the set!");
            }

            return listInfoSet.Add(item);
        }

        public void Clear()
        {
            listInfoSet.Clear();
        }

        public bool Contains(ISPListInfo item)
        {
            return listInfoSet.Contains(item);
        }

        public void CopyTo(ISPListInfo[] array, int arrayIndex)
        {
            listInfoSet.CopyTo(array, arrayIndex);
        }

        public bool Remove(ISPListInfo item)
        {
            return listInfoSet.Remove(item);
        }

        public int Count => listInfoSet.Count;

        public bool IsReadOnly => false;
    }
}