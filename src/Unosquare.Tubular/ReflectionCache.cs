namespace Unosquare.Tubular
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class ReflectionCache
    {
        private static readonly ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> DatePropertyCache = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> TypePropertyCache =
            new ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>>();

        private static readonly ConcurrentDictionary<Type, bool> DbQueryCache = new ConcurrentDictionary<Type, bool>();

        public static IEnumerable<PropertyInfo> GetDateProperties(this Type t) => DatePropertyCache.GetOrAdd(t, GetDateTypeProperties);
        
        public static Dictionary<string, PropertyInfo> ExtractProperties(this Type t) => TypePropertyCache.GetOrAdd(t, GetTypeProperties);

        public static Type ExtractPropertyType(this Type t, string name) => TypePropertyCache.GetOrAdd(t, GetTypeProperties)[name].PropertyType;
        
        public static bool IsDbQuery(this Type t) => DbQueryCache.GetOrAdd(t, GetIsDbQuery);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<PropertyInfo> GetDateTypeProperties(Type t) => TypePropertyCache.GetOrAdd(t, GetTypeProperties)
            .Where(x => x.Value.PropertyType == typeof(DateTime) || x.Value.PropertyType == typeof(DateTime?))
            .Select(x => x.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Dictionary<string, PropertyInfo> GetTypeProperties(Type t) => t.GetProperties()
            .Where(p => (Common.PrimitiveTypes.Contains(p.PropertyType) || p.PropertyType.GetType().IsEnum()) && p.CanRead)
            .ToDictionary(k => k.Name, v => v);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool GetIsDbQuery(Type t) => t.GetTypeInfo().IsGenericType &&
                                                    t.GetInterfaces().Any(y => y == typeof(IListSource));
    }
}
