using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using NiftyFramework.Core.Attributes;
using UnityEditor;
using UnityEngine;

namespace NiftyFramework.Core
{
    public class PrefStorage<TStorageClass>
    {
        private string _appKey;
        private string _basePath;
        private Version _version;
        private Type _serializedType;
        
        public PrefStorage(Version version)
        {
            _appKey = GetMd5Hash(Application.dataPath);
            _version = version;
            _basePath = nameof(TStorageClass);
            _serializedType = typeof(TStorageClass);
        }

        private string GetPath(string key)
        {
            return $"{_appKey}.{_basePath}.{key}";
        }
        
        private static string GetMd5Hash(string input)
        {
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                sb.Append(data[i].ToString("x2"));
            return sb.ToString();
        }

        public void Save()
        {
            FieldInfo[] fieldInfoList = _serializedType.GetFields(BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo fieldInfo in fieldInfoList)
            {
                if (TryGetAttribute<SerializedPref>(fieldInfo, out var attribute))
                {
                    string key = fieldInfo.Name;
                    string path = GetPath(key);
                    if (fieldInfo.FieldType == typeof(string))
                    {
                        EditorPrefs.SetString(path, (string)fieldInfo.GetValue(null));
                    }
                    else if (fieldInfo.FieldType == typeof(int) ||
                             fieldInfo.FieldType == typeof(sbyte) ||
                             fieldInfo.FieldType == typeof(byte) ||
                             fieldInfo.FieldType == typeof(ushort) ||
                             fieldInfo.FieldType == typeof(short))
                    {
                        EditorPrefs.SetInt(path, (int)fieldInfo.GetValue(null));
                    }
                    else if (fieldInfo.FieldType == typeof(float))
                    {
                        EditorPrefs.SetFloat(path, (float)fieldInfo.GetValue(null));
                    }
                    else if (fieldInfo.FieldType == typeof(char))
                    {
                        EditorPrefs.SetInt(path, Convert.ToInt32(fieldInfo.GetValue(null)));
                    }
                    else if (fieldInfo.FieldType == typeof(bool))
                    {
                        EditorPrefs.SetBool(path,(bool)fieldInfo.GetValue(null)); 
                    }
                    else
                    {
                        Debug.Log($"nameof(PrefStorage<TStorageClass>) Unhandled type " + fieldInfo.FieldType);
                    }
                }
            }
        }

        public void Load()
        {
            FieldInfo[] fieldInfoList = _serializedType.GetFields(BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo fieldInfo in fieldInfoList)
            {
                if (TryGetAttribute<SerializedPref>(fieldInfo, out var attribute))
                {
                    string key = fieldInfo.Name;
                    string path = GetPath(key);
                    if (fieldInfo.FieldType == typeof(string))
                    {
                        string stringValue = EditorPrefs.GetString(path, (string)fieldInfo.GetValue(null));
                        fieldInfo.SetValue(stringValue, null);
                    }
                    else if (fieldInfo.FieldType == typeof(int))
                    {
                        int intValue = EditorPrefs.GetInt(path, (int)fieldInfo.GetValue(null));
                        fieldInfo.SetValue(intValue, null);
                    }
                    else if (fieldInfo.FieldType == typeof(sbyte))
                    {
                        sbyte sbyteValue = Convert.ToSByte(EditorPrefs.GetInt(path, (int)fieldInfo.GetValue(null)));
                        fieldInfo.SetValue(sbyteValue, null);
                    }
                    else if (fieldInfo.FieldType == typeof(byte))
                    {
                        byte byteValue = Convert.ToByte(EditorPrefs.GetInt(path, (int)fieldInfo.GetValue(null)));
                        fieldInfo.SetValue(byteValue, null);
                    }
                    else if (fieldInfo.FieldType == typeof(ushort))
                    {
                        ushort ushortValue = Convert.ToUInt16(EditorPrefs.GetInt(path, (int)fieldInfo.GetValue(null)));
                        fieldInfo.SetValue(ushortValue, null);
                    }
                    else if (fieldInfo.FieldType == typeof(short))
                    {
                        short shortValue = Convert.ToInt16(EditorPrefs.GetInt(path, (int)fieldInfo.GetValue(null)));
                        fieldInfo.SetValue(shortValue, null);
                    }
                    else if (fieldInfo.FieldType == typeof(float))
                    {
                        float floatValue = EditorPrefs.GetFloat(path, (float)fieldInfo.GetValue(null));
                        fieldInfo.SetValue(floatValue, null);
                    }
                    else if (fieldInfo.FieldType == typeof(char))
                    {
                        char charValue =  Convert.ToChar(EditorPrefs.GetInt(path,(int)fieldInfo.GetValue(null)));
                        fieldInfo.SetValue(charValue, null);
                    }
                    else if (fieldInfo.FieldType == typeof(bool))
                    {
                        bool boolValue = EditorPrefs.GetBool(path, (bool)fieldInfo.GetValue(null));
                        fieldInfo.SetValue(boolValue, null);
                    }
                    else
                    {
                        Debug.Log($"nameof(PrefStorage<TStorageClass>) Unhandled type " + fieldInfo.FieldType);
                    }
                }
            }
        }
        
        private static bool TryGetAttribute<TAttributeData>(FieldInfo fieldInfo, out TAttributeData data)
            where TAttributeData : System.Attribute
        {
            if (fieldInfo.IsDefined(typeof(TAttributeData), true))
            {
                data = fieldInfo.GetCustomAttribute<TAttributeData>();
                return true;
            }

            data = null;
            return false;
        }
    }
}