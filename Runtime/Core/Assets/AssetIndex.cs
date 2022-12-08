using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace NiftyFramework.Core.Assets
{
    public class AssetIndex<TBaseType> : ScriptableObject, ISerializationCallbackReceiver where TBaseType : ScriptableObject
    {
        [SerializeField] private string _directory;
        [SerializeField] private TBaseType[] _references;
        public ReadOnlyDictionary<Type, IList<TBaseType>> Map { get; private set; }
        public string Directory => _directory;

        public void OnBeforeSerialize()
        {
            //UpdateReferences();
        }

        public void OnEnable()
        {
            #if UNITY_EDITOR
            if (string.IsNullOrEmpty(_directory))
            {
                string assetPath = AssetDatabase.GetAssetPath(this);
                _directory = Path.GetDirectoryName(assetPath);
                
            }
            if (!string.IsNullOrEmpty(_directory))
            {
                UpdateReferences();
            }
            #endif
            
        }

        public bool TryGet<TAsset>(out TAsset asset)
        {
            Type key = typeof(TAsset);
            if (Map.TryGetValue(key, out IList<TBaseType> storedItems))
            {
                var first = storedItems.First();
                if (first is TAsset storedAsset)
                {
                    asset = storedAsset;
                    return true;
                }
                else
                {
                    Debug.LogWarning($"Asset {first} stored at {key} cannot cast to given type {typeof(TAsset)}");
                }
            }
            asset = default;
            return false;
        }

        public bool TryGet<TAsset>(out List<TAsset> assetList) where TAsset : ScriptableObject
        {
            Type key = typeof(TAsset);
            if (Map.TryGetValue(key, out IList<TBaseType> storedItems))
            {
                IList<TBaseType> list = storedItems;
                var castedList = list.Cast<TAsset>();
                assetList = castedList.ToList();
                return true;
            }

            assetList = null;
            return false;
        }

        #if UNITY_EDITOR
        [ContextMenu("UpdateReferences")]
        public void UpdateReferences()
        {
            List<TBaseType> newAssets = new List<TBaseType>();
            string[] assetGuids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { _directory });
            foreach (string guid in assetGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(".asset"))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<TBaseType>(path);
                    if (asset != null && asset != this)
                    {
                        newAssets.Add(asset);
                    }
                }
            }

            _references = newAssets.ToArray();
            CacheMap();
        }
        #endif

        protected void CacheMap()
        {
            var map = new Dictionary<Type, IList<TBaseType>>();
            foreach (var item in _references)
            {
                if (item == this)
                {
                    continue;
                }

                Type key = item.GetType();
                if (map.TryGetValue(key, out IList<TBaseType> list))
                {
                    list.Add(item);
                }
                else
                {
                    map.Add(key, new List<TBaseType> { item });
                }
            }

            Map = new ReadOnlyDictionary<Type, IList<TBaseType>>(map);
        }

        public void OnAfterDeserialize()
        {
            CacheMap();
        }
    }
}