using System;
using System.Collections.Generic;
using UnityEditor;

namespace NiftyFramework.Core.Utils
{
    public class AssetUtils
    {
        public static bool TryLoad<TAsset>(string fileName, out TAsset asset) where TAsset : UnityEngine.Object
        {
            string searchString = $"t:{typeof(TAsset).Name} {fileName}";
            string[] assetGuids = AssetDatabase.FindAssets(searchString);
            if (assetGuids.Length > 0)
            {
                string firstAssetGUID = assetGuids[0];
                string firstAssetPath = AssetDatabase.GUIDToAssetPath(firstAssetGUID);
                asset = AssetDatabase.LoadAssetAtPath<TAsset>(firstAssetPath);
                return true;
            }
            asset = default;
            return false;
        }
        
        public static bool TryLoad<TAsset>(string fileName, out List<TAsset> assetList, Predicate<TAsset> assetPredicate, Predicate<string> guidPredicate, Predicate<string> pathPredicate) where TAsset : UnityEngine.Object
        {
            string searchString = $"t:{typeof(TAsset).Name} {fileName}";
            string[] assetGuids = AssetDatabase.FindAssets(searchString);
            assetList = new List<TAsset>();
            foreach (var guid in assetGuids)
            {
                if (guidPredicate != null && !guidPredicate(guid))
                {
                    continue;
                }
                string assetPath= AssetDatabase.GUIDToAssetPath(guid);
                if (pathPredicate != null && !pathPredicate(assetPath))
                {
                    continue;
                }
                TAsset asset =  AssetDatabase.LoadAssetAtPath<TAsset>(assetPath);
                if (assetPredicate != null && !assetPredicate(asset))
                {
                    continue;
                }
                assetList.Add(asset);
            }
            return assetList.Count > 0;
        }
    }
}