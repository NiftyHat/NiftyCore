using UnityEditor;
using UnityEngine;

namespace NiftyFramework.SerializedDictionary
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializedDictionaryPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty spEntries = property.FindPropertyRelative("_entries");
            if (spEntries.isArray)
            {
                int len = spEntries.arraySize;
                for (int i = 0; i < len; i++)
                {
                    EditorGUI.PropertyField(position, spEntries.GetArrayElementAtIndex(i));
                }
            }
            base.OnGUI(position, property, label);
        }
    }
}