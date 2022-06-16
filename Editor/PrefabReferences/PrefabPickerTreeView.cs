using System;
using System.Collections.Generic;
using System.IO;
using NiftyFramework.Core.PrefabReferences;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace NiftyFramework.PrefabReferences
{
    public class PrefabPickerTreeView : UnityEditor.IMGUI.Controls.TreeView
    {
        public class PrefabPickerTreeViewItem : TreeViewItem
        {
            internal string path;

            public GameObject Load()
            {
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
        }

        public event Action<IEnumerable<GameObject>> OnSelectionChange;
        public event Action<GameObject> OnDoubleClickItem;
        protected PrefabReference _prefabReference;
        protected Type _componentType;

        public PrefabPickerTreeView(TreeViewState treeViewState, PrefabReference prefabReference)
            : base(treeViewState)
        {
            _prefabReference = prefabReference;
            Reload();
        }
        
        public PrefabPickerTreeView(TreeViewState treeViewState, Type componentType)
            : base(treeViewState)
        {
            _componentType = componentType;
            Reload();
        }

        protected override void DoubleClickedItem(int selectedId)
        {
            var rows = FindRows(new int[] { selectedId });
            if (rows.Count == 1)
            {
                var itemRow = rows[0];
                if (itemRow is PrefabPickerTreeViewItem treeViewItem)
                {
                    OnDoubleClickItem?.Invoke(treeViewItem.Load());
                }
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            /*
            var rows = FindRows(selectedIds);
            var selectedAssets = rows.Select(item =>
            {
                if (item is AssetTreeViewItem treeViewItem)
                {
                    return treeViewItem.asset;
                }
                return null;
            });
            OnSelectionChange?.Invoke(selectedAssets);
            base.SelectionChanged(selectedIds);*/
        }

        private static string[] FindAssetGUIDs(string[] folders)
        {
            string filter = $"t:{nameof(GameObject)}";
            if (folders != null && folders.Length > 0)
            {
                return AssetDatabase.FindAssets(filter, folders);
            }
            return FindAssetGUIDs();
        }

        private static string[] FindAssetGUIDs(string folder)
        {
            if (folder != null)
            {
                return FindAssetGUIDs(new[] { folder });
            }
            return FindAssetGUIDs();
        }

        private static string[] FindAssetGUIDs()
        {
            string filter = $"t:{nameof(GameObject)}";
            return AssetDatabase.FindAssets(filter);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            return base.BuildRows(root);
        }

        protected bool ValidateHasComponent(string assetPath)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            Core.PrefabReferences.PrefabReference prefabReference = null;
            if (prefabReference.ValidatePrefab(asset))
            {
                return true;
            }
            return false;
            /*
            if (asset != null)
            {
                TAsset requiredComponent = asset.GetComponent<TAsset>();
                if (requiredComponent != null)
                {
                    return true;
                }
            }
            return false;*/
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
            string[] assetGUIDs = FindAssetGUIDs();
            int id = 0;
            foreach (var guid in assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileName(assetPath);
                var typeIcon = AssetDatabase.GetCachedIcon(assetPath);
                id++;
                bool hasComponent = false;
                if (_prefabReference != null)
                {
                    var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    hasComponent = _prefabReference.ValidatePrefab(asset);
                }
                else if (_componentType != null)
                {
                    var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    hasComponent = asset.GetComponent(_componentType);
                }
                
                if (hasComponent)
                {
                    PrefabPickerTreeViewItem typeItem = new PrefabPickerTreeViewItem
                        { id = id, depth = 1, displayName = $"{fileName}", path = assetPath };
                    typeItem.icon = typeIcon as Texture2D;
                    root.AddChild(typeItem);
                }
            }
            return root;
        }
    }
}