using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace RuntimeProperties
{
    public static class RuntimeBindingUtility
    {
        public static List<IRuntimePropertyBinder> GetBindings(object prototype)
        {
            if (prototype == null)
            {
                return null;
            }
            System.Type baseType = prototype.GetType();
            if (TryGetFields(baseType, out var fieldInfoList))
            {
                List<IRuntimePropertyBinder> bindings = new List<IRuntimePropertyBinder>();
                for (int i = 0; i < fieldInfoList.Length; i++)
                {
                    FieldInfo field = fieldInfoList[i];
                    bindings.Add(new RuntimePropertyFieldBinding(field, prototype));
                }
                return bindings;
            }
            return null;
        }

        public static bool TryGetFields(System.Type baseType, out FieldInfo[] fieldInfoList)
        {
            //BindingFlags.Public | BindingFlags.DeclaredOnly
            fieldInfoList = baseType.GetFields();
            return fieldInfoList.Length > 0;
        }

        public static List<IRuntimePropertyBinder> GetBindings(IList list, string fieldName)
        {
            int len = list.Count;
            System.Type elementType = list.GetType().GetElementType();
            List<IRuntimePropertyBinder> bindings = new List<IRuntimePropertyBinder>();
            for (int i = 0; i < len; i++)
            {
                bindings.Add(new RuntimePropertyListItemBinding(list, i, elementType, fieldName));
            }
            return bindings;
        }
    }
}