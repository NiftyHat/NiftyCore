using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NiftyFramework.Core.Services;

namespace NiftyFramework.Services
{
    public class ServiceImplementationProvider<TBase> where TBase : NiftyService
    {
        public class ItemData
        {
            private readonly bool _isObsolete;
            private readonly string _displayName;
            private string _info;
            private Type _itemType;
            public string DisplayName => _displayName;
            public bool IsObsolete => _isObsolete;
            public Type ItemType => _itemType;

            public ItemData(Type itemType)
            {
                _displayName = itemType.Name;
                _itemType = itemType;

                if (TryGetAttribute(itemType, out NiftyServiceAttribute attribute))
                {
                    _info = attribute.Info;
                }
                else
                {
                    _info = "Missing [ScriptableSetItemAttribute] attribute ";
                }

                if (TryGetAttribute(itemType, out ObsoleteAttribute obsoleteAttribute))
                {
                    _isObsolete = true;
                }
            }
        }
        
        private Dictionary<Type, ItemData> _map;
        public ItemData DefaultItem { get; private set; }
        
        public bool IsMultiType => _map != null && _map.Count > 0;
        
        public ServiceImplementationProvider()
        {
            Type baseType = typeof(TBase);

            Type[] assemblyType = baseType.Assembly.GetTypes();

            if (baseType.GetConstructor(Type.EmptyTypes) != null)
            {
                DefaultItem = new ItemData(baseType);
            }

            foreach (Type type in assemblyType)
            {
                if (type.IsSubclassOf(baseType) && type.IsClass && !type.IsAbstract)
                {
                    if (_map == null)
                    {
                        _map = new Dictionary<Type, ItemData>();
                    }
                    _map[type] = new ItemData(type);
                }
            }
        }
        
        private static bool TryGetAttribute<TAttributeData>(Type targetType, out TAttributeData data) where TAttributeData : System.Attribute
        {
            if (targetType.IsDefined(typeof(TAttributeData), true))
            {
                data = targetType.GetCustomAttribute<TAttributeData>();
                return true;
            }
            data = null;
            return false;
        }

        public List<ItemData> GetItems()
        {
            if (IsMultiType)
            {
                return _map.Values.ToList();
            }
            else if (DefaultItem != null)
            {
                return new List<ItemData>(){DefaultItem};
            }
            return null;
        }
    }
}