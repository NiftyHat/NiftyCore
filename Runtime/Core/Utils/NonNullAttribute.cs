namespace NiftyFramework.Core.Utils
{
	using UnityEngine;
	using System.Collections;

#if UNITY_EDITOR
	using UnityEditor;
	using UnityEditor.SceneManagement;
	using System.Reflection;
	using UnityEditor.Callbacks;
#endif

	[System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Class)]
	public class NonNullAttribute : PropertyAttribute
	{
	}

	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class AllowNullAttribute : PropertyAttribute
	{
	}

	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class NonEmptyAttribute : PropertyAttribute
	{
	}

#if UNITY_EDITOR
	internal static class NullFieldGUI
	{
		public static void NullCheckedField(Rect position, SerializedProperty property, GUIContent label,
			bool showWarning)
		{
			if (!showWarning)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			GUI.backgroundColor = Color.red;

			var fillCandidate = FindNonNull.FindObjectToFill(property, out string fillButtonText);
			if (fillCandidate == null)
			{
				EditorGUI.PropertyField(position, property, label);
				GUI.backgroundColor = Color.white;
				return;
			}

			var propertyRect = new Rect
				{ x = position.x, y = position.y, width = position.width - 45, height = position.height };
			var buttonRect = new Rect
				{ x = position.x + propertyRect.width + 8, y = position.y, width = 45 - 8, height = position.height };

			EditorGUI.PropertyField(propertyRect, property, label);

			GUI.backgroundColor = Color.white;
			if (GUI.Button(buttonRect, fillButtonText))
			{
				property.objectReferenceValue = fillCandidate;
			}
		}
	}

	[CustomPropertyDrawer(typeof(Object), useForChildren: true)]
	public class DefaultObjectDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var showWarning = (
				property.propertyType == SerializedPropertyType.ObjectReference
				&& property.objectReferenceValue == null
				&& FindNonNull.ClassHasAttributeOfType(property.serializedObject.targetObject.GetType(),
					typeof(NonNullAttribute))
			);

			NullFieldGUI.NullCheckedField(position, property, label, showWarning);
			EditorGUI.EndProperty();
		}
	}

	[CustomPropertyDrawer(typeof(AllowNullAttribute))]
	public class AllowNullAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.PropertyField(position, property, label);
			EditorGUI.EndProperty();
		}
	}

	[CustomPropertyDrawer(typeof(NonNullAttribute))]
	public class NonNullAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			var showWarning = (property.propertyType == SerializedPropertyType.ObjectReference &&
			                   property.objectReferenceValue == null);
			NullFieldGUI.NullCheckedField(position, property, label, showWarning);
			EditorGUI.EndProperty();
		}
	}

	[CustomPropertyDrawer(typeof(NonEmptyAttribute))]
	public class NonEmptyAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			bool showWarning;

			switch (property.propertyType)
			{
				case SerializedPropertyType.String:
					showWarning = string.IsNullOrEmpty(property.stringValue);
					break;
				case SerializedPropertyType.AnimationCurve:
					showWarning = (property.animationCurveValue == null || property.animationCurveValue.length == 0);
					break;
				case SerializedPropertyType.LayerMask:
					showWarning = (property.intValue == 0);
					break;
				case SerializedPropertyType.ArraySize:
					showWarning = (property.arraySize == 0);
					break;
				case SerializedPropertyType.Color:
					showWarning = (property.colorValue == new Color(0, 0, 0, 0));
					break;
				case SerializedPropertyType.Enum:
					showWarning = (property.enumValueIndex == 0);
					break;
				case SerializedPropertyType.Integer:
					showWarning = (property.intValue == 0);
					break;
				case SerializedPropertyType.Float:
					showWarning = (Mathf.Approximately((float)property.doubleValue, 0));
					break;
				case SerializedPropertyType.Vector2:
					showWarning = property.vector2Value == Vector2.zero;
					break;
				case SerializedPropertyType.Vector3:
					showWarning = property.vector3Value == Vector3.zero;
					break;
				// case SerializedPropertyType.Vector4: showWarning = property.vector4Value == Vector4.zero; break;
				case SerializedPropertyType.Vector2Int:
					showWarning = property.vector2IntValue == Vector2Int.zero;
					break;
				case SerializedPropertyType.Vector3Int:
					showWarning = property.vector3IntValue == Vector3Int.zero;
					break;
				default:
					showWarning = false;
					break;
			}

			NullFieldGUI.NullCheckedField(position, property, label, showWarning);
			EditorGUI.EndProperty();
		}
	}

	internal static class FindNonNull
	{
		[PostProcessScene]
		public static void ScenePostProcess()
		{
			if (FindAllNonNulls())
			{
				if (Application.isPlaying)
				{
					EditorApplication.isPaused = true;
				}
			}
		}

		[MenuItem("Assets/NonNull/Check for unassigned references in current scene")]
		public static bool FindAllNonNulls()
		{
			var anyNulls = false;

			EnumerateAllComponentsInScene((obj, component) =>
			{
				NullCheckComponent(obj, component, ref anyNulls);
			});

			return anyNulls;
		}

		[MenuItem("Assets/NonNull/Check for unassigned references in all scenes in build settings")]
		public static void FindAllNonNullsInAllScenes()
		{
			var scenes = EditorBuildSettings.scenes;

			if (scenes.Length == 0)
			{
				Debug.Log("No scenes in build settings, so no scenes checked.");
				return;
			}

			for (int i = 0; i < scenes.Length; i++)
			{
				var scene = scenes[i];

				if (EditorUtility.DisplayCancelableProgressBar("Checking all scenes", scene.path,
					i / (float)scenes.Length))
				{
					EditorUtility.ClearProgressBar();
					return;
				}

				EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
				FindAllNonNulls();
			}

			EditorUtility.ClearProgressBar();
		}

		private static void NullCheckComponent(GameObject obj, Component component, ref bool anyNulls)
		{
			if (component == null)
			{
				LOGError("Missing script for component", obj);
				anyNulls = true;
				return;
			}

			var componentHasNonNull = ClassHasAttributeOfType(component.GetType(), typeof(NonNullAttribute));
			var fields = component.GetType()
				.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			foreach (var field in fields)
			{
				var isSerialized = FieldHasAttributeOfType(field, typeof(SerializeField));
				var isPublic = FieldAccessIs(field, FieldAttributes.Public);
				if (!isSerialized && !isPublic)
				{
					continue;
				}

				NullCheckField(obj, component, field, componentHasNonNull, ref anyNulls);
				EmptyCheckField(obj, component, field, ref anyNulls);
			}
		}

		private static void EmptyCheckField(GameObject obj, Component component, FieldInfo field, ref bool anyNulls)
		{
			if (!FieldHasAttributeOfType(field, typeof(NonEmptyAttribute)))
			{
				return;
			}

			var fieldValue = field.GetValue(component);

			switch (fieldValue)
			{
				case string value when string.IsNullOrEmpty(value):
					LOGError("Empty string", obj, component, field);
					break;
				case AnimationCurve curve when (curve == null || curve.length == 0):
					LOGError("Empty animation curve", obj, component, field);
					break;
				case LayerMask mask when (fieldValue == null || mask == 0):
					LOGError("Unspecified layer mask", obj, component, field);
					break;
				case System.Array array when (array == null || array.Length == 0):
					LOGError("Empty array", obj, component, field);
					break;
				case IList list when (list == null || list.Count == 0):
					LOGError("Empty list", obj, component, field);
					break;
				case Color color when (fieldValue == null || color == new Color(0, 0, 0, 0)):
					LOGError("No color", obj, component, field);
					break;
				default:
				{
					if (fieldValue != null && fieldValue.GetType().IsEnum && ((int)fieldValue) == 0)
					{
						LOGError("Empty enum value", obj, component, field);
					}
					else switch (fieldValue)
					{
						case int i when i == 0:
							LOGError("Zero integer value", obj, component, field);
							break;
						case float value when Mathf.Approximately(value, 0):
							LOGError("Zero float value", obj, component, field);
							break;
						case double value when value == 0:
							LOGError("Zero double value", obj, component, field);
							break;
						default:
						{
							if (IsEmptyVector(fieldValue))
							{
								LOGError("Empty vector value", obj, component, field);
							}
							else
							{
								return;
							}

							break;
						}
					}

					break;
				}
			}

			anyNulls = true;
		}

		static bool IsEmptyVector(object value)
		{
			if (value is Vector2 vector2 && vector2 == Vector2.zero)
			{
				return true;
			}

			if (value is Vector2Int vector2Int && vector2Int == Vector2Int.zero)
			{
				return true;
			}

			if (value is Vector3 vector3 && vector3 == Vector3.zero)
			{
				return true;
			}

			if (value is Vector3Int vector3Int && vector3Int == Vector3Int.zero)
			{
				return true;
			}

			if (value is Vector4 vector4 && vector4 == Vector4.zero)
			{
				return true;
			}

			return false;
		}

		private static void NullCheckField(GameObject obj, Component component, FieldInfo field, bool componentHasNonNull,
			ref bool anyNulls)
		{
			if (!ShouldNullCheckField(field, componentHasNonNull))
			{
				return;
			}

			var fieldValue = field.GetValue(component);

			if (fieldValue is UnityEngine.Object unityObject)
			{
				if (unityObject != null)
				{
					return;
				}
			}
			else
			{
				if (!object.ReferenceEquals(fieldValue, null))
				{
					return;
				}
			}

			LOGError("Missing reference", obj, component, field);
			anyNulls = true;
		}

		private static bool ShouldNullCheckField(FieldInfo field, bool componentHasNonNull)
		{
			if (!field.FieldType.IsClass)
			{
				return false;
			}

			if (!componentHasNonNull)
			{
				if (!FieldHasAttributeOfType(field, typeof(NonNullAttribute)))
				{
					return false;
				}
			}
			else
			{
				if (FieldHasAttributeOfType(field, typeof(AllowNullAttribute)))
				{
					return false;
				}
			}

			return true;
		}

		public static bool ClassHasAttributeOfType(System.Type classType, System.Type ofType)
		{
			return (classType.GetCustomAttributes(ofType, false).Length > 0);
		}

		private static bool FieldAccessIs(FieldInfo field, FieldAttributes attribute)
		{
			return ((field.Attributes & FieldAttributes.FieldAccessMask) == attribute);
		}

		private static bool FieldHasAttributeOfType(FieldInfo field, System.Type type)
		{
			return (field.GetCustomAttributes(type, false).Length > 0);
		}

		private static void EnumerateAllComponentsInScene(System.Action<GameObject, Component> callback)
		{
			EnumerateAllGameObjectsInScene(obj =>
			{
				var components = obj.GetComponents<Component>();
				foreach (var comp in components)
				{
					callback(obj, comp);
				}
			});
		}

		private static void EnumerateAllGameObjectsInScene(System.Action<GameObject> callback)
		{
			var rootObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();

			foreach (var rootObject in rootObjects)
			{
				EnumerateChildrenOf(rootObject, callback);
			}
		}

		private static void EnumerateChildrenOf(GameObject obj, System.Action<GameObject> callback)
		{
			callback(obj);

			for (var i = 0; i < obj.transform.childCount; i++)
			{
				EnumerateChildrenOf(obj.transform.GetChild(i).gameObject, callback);
			}
		}

		private static FieldInfo GETField(string propertyPath, System.Type fromType)
		{
			while (true)
			{
				var field = fromType.GetField(propertyPath, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
				if (field != null)
				{
					return field;
				}

				var baseType = fromType.BaseType;
				if (baseType != null && baseType != typeof(object))
				{
					fromType = baseType;
					continue;
				}

				return null;
			}
		}

		public static Object FindObjectToFill(SerializedProperty property, out string actionName)
		{
			actionName = null;
			if (property.propertyType != SerializedPropertyType.ObjectReference)
			{
				return null;
			}

			if (property.propertyPath.Contains(".Array"))
			{
				return null;
			}

			var objectType = property.serializedObject.targetObject.GetType();
			var field = GETField(property.propertyPath, fromType: objectType);
			if (field == null)
			{
				return null;
			}

			var fieldType = field.FieldType;

			if (fieldType.IsSubclassOf(typeof(Component)))
			{
				var component = property.serializedObject.targetObject as Component;
				if (component != null)
				{
					var components = component.GetComponents(fieldType);
					if (components.Length == 1)
					{
						actionName = "This";
						return components[0];
					}
				}

				actionName = "Fill";
				return FindSceneObjectToFill(fieldType);
			}

			if (fieldType.IsSubclassOf(typeof(ScriptableObject)))
			{
				actionName = "Fill";
				return FindAssetObjectToFill(fieldType);
			}

			return null;
		}

		private static Object FindSceneObjectToFill(System.Type fieldType)
		{
			Object objectInScene = null;

			var rootObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();
			foreach (var gameObject in rootObjects)
			{
				var candidates = gameObject.GetComponentsInChildren(fieldType, includeInactive: true);
				if (candidates.Length > 1)
				{
					return null;
				}

				if (candidates.Length == 1 && objectInScene != null)
				{
					return null;
				}

				if (candidates.Length == 1)
				{
					objectInScene = candidates[0];
				}
			}

			return objectInScene;
		}

		private static Object FindAssetObjectToFill(System.Type fieldType)
		{
			var objectsInAssets = AssetDatabase.FindAssets("t:" + fieldType.Name);
			if (objectsInAssets.Length != 1)
			{
				return null;
			}

			var assetId = objectsInAssets[0];
			var path = AssetDatabase.GUIDToAssetPath(assetId);
			return AssetDatabase.LoadAssetAtPath(path, fieldType);
		}

		static void LOGError(string error, GameObject obj, Component component, FieldInfo field)
		{
			Debug.LogError(
				error + " for " + field.Name + " in " + component.GetType().Name + " on " + obj.name + " in scene " +
				EditorSceneManager.GetActiveScene().name, component);
		}

		static void LOGError(string error, GameObject obj)
		{
			Debug.LogError(error + " on " + obj.name + " in scene " + EditorSceneManager.GetActiveScene().name, obj);
		}
	}
#endif
}