using NiftyFramework.Core.PrefabReferences;
using UnityEditor;
using UnityEngine;

namespace NiftyFramework.PrefabReferences
{
    [CustomPropertyDrawer(typeof(PrefabReferenceAttribute))]
    public class PrefabReferencePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            PrefabReferenceAttribute attribute = this.attribute as PrefabReferenceAttribute;
            
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);

            Rect rectButtonSelect = new Rect(position.x, position.y, 80, position.height);
            Rect rectObject = new Rect(rectButtonSelect.x, position.y, position.width - rectButtonSelect.width,
                position.height);
            

            if (GUI.Button(rectObject, "Select"))
            {
                PrefabPickerEditorWindow.Init(attribute.Type);
            }

            if (property != null)
            {
                EditorGUI.ObjectField(position,property);
            }
            else
            {
                EditorGUI.LabelField(position, "NULL");
            }
           
            EditorGUI.EndProperty();
            
            /*
            SerializedProperty spGameObject = property.FindPropertyRelative("_prefab");

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);

            Rect rectButtonSelect = new Rect(position.x, position.y, 80, position.height);
            Rect rectObject = new Rect(rectButtonSelect.x, position.y, position.width - rectButtonSelect.width, position.height)
            

            if (GUI.Button(rectObject, "Select"))
            {
                property.serializedObject.
            }

            if (spGameObject != null)
            {
                EditorGUI.ObjectField(position,spGameObject);
            }
            else
            {
                EditorGUI.LabelField(position, "NULL");
            }
           
            EditorGUI.EndProperty();*/
        }
    }
}