using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace MIP.SharePoint.API.Helper
{
    internal static class AttributeHelper
    {
        private static readonly ConcurrentDictionary<MemberInfo, ConcurrentDictionary<Type, Attribute>> attributeCache = new ConcurrentDictionary<MemberInfo, ConcurrentDictionary<Type, Attribute>>();

        internal static TAttribute GetAttribute<TAttribute>(MemberInfo memberInfo) where TAttribute : Attribute
        {
            if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));

            TAttribute attribute;
            var isContainedInCache = TryGetAttributeFromCache(memberInfo, out attribute);

            if (isContainedInCache)
            {
                return attribute;
            }

            attribute = memberInfo.GetCustomAttribute<TAttribute>();
            SetAttributeCache(memberInfo, typeof(TAttribute), attribute);

            return attribute;
        }


        private static bool TryGetAttributeFromCache<TAttribute>(MemberInfo memberInfo, out TAttribute attribute) where TAttribute : Attribute
        {
            var cacheForMemberInfo = attributeCache.GetOrAdd(memberInfo, (x) => new ConcurrentDictionary<Type, Attribute>());
            var isContainedInCache = cacheForMemberInfo.TryGetValue(typeof(TAttribute), out var attr);

            attribute = isContainedInCache ? (TAttribute)attr : null;

            return isContainedInCache;
        }

        private static void SetAttributeCache(MemberInfo memberInfo, Type attributeType, Attribute attributeValue)
        {
            var cacheForMemberInfo = attributeCache.GetOrAdd(memberInfo, (x) => new ConcurrentDictionary<Type, Attribute>());
            cacheForMemberInfo.TryAdd(attributeType, attributeValue);
        }
    }
}