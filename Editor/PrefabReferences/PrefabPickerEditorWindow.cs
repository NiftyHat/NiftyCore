using System;
using System.Collections.Generic;
using NiftyFramework.Core.PrefabReferences;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace NiftyFramework.PrefabReferences
{
    public class PrefabPickerEditorWindow : EditorWindow
    {
        private PrefabPickerTreeView _treeView;
        private TreeViewState _treeViewState;
        private PrefabReference _prefabReference;
        private Type _componentType;
        private string _searchString;
        
        public static void Init(PrefabReference prefabReference)
        {
            var window = (PrefabPickerEditorWindow)GetWindow(typeof(PrefabPickerEditorWindow));
            window.SetPrefabReference(prefabReference);
            window.titleContent = new GUIContent("Prefab Reference Picker");
            window.Show();
        }
        public static void Init(Type type)
        {
            var window = (PrefabPickerEditorWindow)GetWindow(typeof(PrefabPickerEditorWindow));
            window.SetType(type);
            window.titleContent = new GUIContent("Prefab Reference Picker");
            window.Show();
            window.InitTreeView();
        }

        private void SetPrefabReference(PrefabReference prefabReference)
        {
            _prefabReference = prefabReference;
        }

        private void SetType(Type componentType)
        {
            _componentType = componentType;
        }

        private void OnEnable()
        {
            InitTreeView();
        }

        private void OnGUI()
        {
            DrawTree();
        }

        private void DrawTree()
        {
            if (_treeView != null)
            {
                Rect treeRect = EditorGUILayout.GetControlRect(true, _treeView.totalHeight);
                _treeView.searchString = _searchString;
                _treeView.OnGUI(treeRect);
            }
        }

        public void InitTreeView()
        {
            if (_treeViewState == null)
            {
                _treeViewState = new TreeViewState ();
            }

            if (_prefabReference != null)
            {
                _treeView = new PrefabPickerTreeView(_treeViewState, _prefabReference);
            }
            else if (_componentType != null)
            {
                _treeView = new PrefabPickerTreeView(_treeViewState, _componentType);
            }

            if (_treeView != null)
            {
                _treeView.OnSelectionChange += HandlePrefabSelected;
                _treeView.OnDoubleClickItem += HandlePrefabDoubleClick;
            }

            EditorApplication.projectChanged += HandleProjectChange;
        }

        private void HandleProjectChange()
        {
            _treeViewState = new TreeViewState ();
            if (_treeView != null)
            {
                _treeView.Reload();
            }
        }

        private void HandlePrefabDoubleClick(GameObject obj)
        {
            _prefabReference.SetReference(obj);
        }

        private void HandlePrefabSelected(IEnumerable<GameObject> obj)
        {
            //throw new NotImplementedException();
        }


    }
}