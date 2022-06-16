using UnityEditor;
using UnityEngine;

namespace NiftyFramework.Core
{
    [CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
    public class SpritePreviewPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            if (prop.objectReferenceValue != null)
            {
                return _textureSize;
            }
            return base.GetPropertyHeight(prop, label);
        }

        private const float _textureSize = 65;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, prop);

            if (prop.objectReferenceValue != null)
            {
                //position.width = EditorGUIUtility.labelWidth;
                //GUI.Label(position, prop.displayName);
                Rect imageRect = EditorGUI.PrefixLabel(position, new GUIContent(prop.displayName));

                //imageRect.x += position.width;
                imageRect.width = _textureSize;
                imageRect.height = _textureSize;

                prop.objectReferenceValue = EditorGUI.ObjectField(imageRect, prop.objectReferenceValue, typeof(Sprite), false);
            }
            else
            {
                EditorGUI.PropertyField(position, prop, true);
            }

            EditorGUI.EndProperty();
        }
    }
}