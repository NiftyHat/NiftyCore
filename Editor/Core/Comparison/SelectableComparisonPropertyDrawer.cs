using System;
using UnityEditor;
using UnityEngine;


namespace NiftyFramework.Core.Comparison
{
    [CustomPropertyDrawer(typeof(SelectableComparison<>))]
    public class SelectableComparisonPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var selectedRect = new Rect(position.x, position.y, 40, position.height);

            SerializedProperty spSelected = property.FindPropertyRelative("_selected");
            
            
            Color defaultColor = GUI.color;
            if (string.IsNullOrEmpty(spSelected.stringValue))
            {
                GUI.color = Color.red;
            }
            if (EditorGUI.DropdownButton(selectedRect,
                new GUIContent(spSelected.stringValue), FocusType.Keyboard))
            {
                GenericMenu menu = new GenericMenu();
                foreach (var item in NiftyComparison.SerializableValues)
                {
                    menu.AddItem(new GUIContent(item), spSelected.stringValue == item, HandleItemSelected,  item);
                }
            
                void HandleItemSelected(object userData)
                {
                    spSelected.stringValue = (string)userData;
                    spSelected.serializedObject.ApplyModifiedProperties();
                }
                
                menu.DropDown(selectedRect);
            }
            GUI.color = defaultColor;

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
            //base.OnGUI(position, property, label);
        }
    }
}