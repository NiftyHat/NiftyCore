using NiftyFramework.Core.Data;
using UnityEditor;
using UnityEngine;


namespace Core.Data
{
    [CustomPropertyDrawer(typeof(SerializedKeyValuePair<,>))]
    public class SerializedKeyValuePairDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty spKey = property.FindPropertyRelative("Key");
            SerializedProperty spValue = property.FindPropertyRelative("Value");
            if (spValue != null && spValue.hasVisibleChildren)
            {
                //Rect rectKey = new Rect(position.x, position.y, position.width * 0.9f, position.height);
                //Rect rectFoldout = new Rect(rectKey.xMax, position.y, position.width * 0.1f, position.height);
                EditorGUI.Foldout(position, false, spKey.stringValue);
                //Rect rectKey = new Rect(position.x, position.y, position.width, position.height);
            }
            else if (spKey != null)
            {
                Rect rectKey = new Rect(position.x, position.y, position.width * 0.5f, position.height);
                Rect rectValue = new Rect(rectKey.xMax, position.y, position.width - rectKey.width, position.height);
                //EditorGUI.PropertyField(rectKey, spKey);
                //EditorGUI.PropertyField(rectValue, spValue);
            }

            base.OnGUI(position, property, label);
        }
    }
}