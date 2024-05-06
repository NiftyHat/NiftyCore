using System;
using System.Collections;
using System.Reflection;
using UnityEngine.UIElements;

namespace RuntimeProperties
{
    public abstract class RuntimePropertyBinding : IRuntimePropertyBinder
    {
        public string Label { get; protected set; }
        public abstract void Set(object value);

        public abstract object Get();

        public Type DataType { get; protected set; }

        public virtual VisualElement GetView()
        {
            if (DataType.IsArray)
            {
                return new RuntimePropertyElement_List(this);
            }
            
            if (DataType == typeof(string))
            {
                return new RuntimePropertyElement_Field<string,TextField> (this);
            }

            if (DataType == typeof(bool))
            {
                return new RuntimePropertyElement_Field<bool, Toggle>(this);
            }
            
            if (DataType == typeof(int))
            {
                return new RuntimePropertyElement_Field<int, IntegerField>(this);
            }
            
            if (DataType == typeof(uint))
            {
                return new RuntimePropertyElement_Field<uint, UnsignedIntegerField>(this);
            }
            
            if (DataType == typeof(long))
            {
                return new RuntimePropertyElement_Field<long, LongField>(this);
            }
            
            if (DataType == typeof(ulong))
            {
                return new RuntimePropertyElement_Field<ulong, UnsignedLongField>(this);
            }
            
            if (DataType == typeof(double))
            {
                return new RuntimePropertyElement_Field<double, DoubleField>(this);
            }
            
            return new RuntimePropertyElement_Missing(Label, DataType);
        }
    }

    public interface IRuntimePropertyBinder
    {
        public string Label { get; }
        public void Set(object value);

        public object Get();
        public VisualElement GetView();
    }
    
    public class RuntimePropertyFieldBinding : RuntimePropertyBinding, IRuntimePropertyBinder
    {
        private FieldInfo _fieldInfo;
        private readonly object _data;
        
        public RuntimePropertyFieldBinding(FieldInfo fieldInfo, object data)
        {
            Label = fieldInfo.Name;
            DataType = fieldInfo.FieldType;
            _fieldInfo = fieldInfo;
            _data = data;
        }

        public override void Set(object value)
        {
            _fieldInfo.SetValue(_data, value);
        }

        public override object Get()
        {
            return _fieldInfo.GetValue(_data);
        }
    }
    
    public class RuntimePropertyListItemBinding : RuntimePropertyBinding,  IRuntimePropertyBinder
    {
        protected int _index;
        protected IList _list;
        
        public RuntimePropertyListItemBinding(IList list, int index, System.Type dataType = null, string propertyName = null)
        {
            if (dataType == null)
            {
                dataType = list.GetType().GetGenericArguments()[0];
            }
            DataType = dataType;
            _index = index;
        }

        public override object Get()
        {
            return _list[_index];
        }

        public override void Set(object value)
        {
            _list[_index] = value;
        }
    }
}