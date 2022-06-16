using NiftyFramework.Core.Data;
using UnityEditor;
using UnityEngine;

namespace Core.Data
{
    [CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalPropertyDrawer : PropertyDrawer
    {
        /*
         * Property drawer for optional properties. Based on examples from https://gist.github.com/aarthificial
         * requires unity 2020.1 or later.
         */
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("_value");
            var enabledProperty = property.FindPropertyRelative("_enabled");

            position.width -= 24;
            EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);
            {
                EditorGUI.PropertyField(position, valueProperty, label, true);
            }
            EditorGUI.EndDisabledGroup();
            
            position.x += position.width - 8;
            position.height = EditorGUI.GetPropertyHeight(enabledProperty);
            position.width = 24;
            EditorGUI.PropertyField(position, enabledProperty, GUIContent.none);

        }
    }
}