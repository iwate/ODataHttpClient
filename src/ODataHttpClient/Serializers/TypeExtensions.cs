using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;

namespace ODataHttpClient.Serializers
{
    public static class TypeExtensions
    {
        private static readonly Type _ienumerable = typeof(IEnumerable);
        private static readonly Type[] _ignoreMultipes = new[] { typeof(string) };
        private static readonly ConcurrentDictionary<Type, Type[]> _interfaces = new ConcurrentDictionary<Type, Type[]>();
        private static readonly ConcurrentDictionary<Type, Type> _items = new ConcurrentDictionary<Type, Type>();
        public static Type[] GetCachedInterfaces(this Type type)
        {
            if (!_interfaces.ContainsKey(type))
            {
                _interfaces[type] = type.GetInterfaces();
            }
            return _interfaces[type];
        }
        public static bool IsMultiple(this Type type)
        {
            return !_ignoreMultipes.Contains(type) && type.GetCachedInterfaces().Contains(_ienumerable);
        }
        public static Type GetItemType(this Type type)
        {
            if (!_items.ContainsKey(type))
            {
                if (type.IsArray)
                {
                    var t = type.GetCachedInterfaces().Where(i => i.GenericTypeArguments.Length > 0 &&i.GetInterfaces().Any(e => e == _ienumerable)).FirstOrDefault();
                    _items[type] = t?.GetGenericArguments().FirstOrDefault();
                }
                else
                {
                    _items[type] = type.GetGenericArguments().FirstOrDefault();
                }
            }
            return _items[type];
        }
    }
}