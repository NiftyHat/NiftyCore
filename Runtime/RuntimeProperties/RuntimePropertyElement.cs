using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace RuntimeProperties
{
    public abstract class RuntimePropertyElement_Field<TData> : VisualElement
    {
        public readonly BaseField<TData> Field;
        public readonly IRuntimePropertyBinder Binding;
        
        protected RuntimePropertyElement_Field([NotNull] IRuntimePropertyBinder binding)
        {
            Binding = binding;
            Field = GetFieldView(Binding);
            Field.RegisterValueChangedCallback(HandleValueChanged);
            Add(Field);
        }

        private void HandleValueChanged(ChangeEvent<TData> evt)
        {
            Binding.Set(evt.newValue);
        }

        public abstract BaseField<TData> GetFieldView(IRuntimePropertyBinder binding);
    }

    public class RuntimePropertyElement_List : VisualElement
    {
        public readonly RuntimePropertyBinding Binding;
        public System.Type ItemType;

        public RuntimePropertyElement_List([NotNull] RuntimePropertyBinding binding)
        {
            Binding = binding;
            ItemType = Binding.DataType.GetGenericTypeDefinition();
        }
    }
    
    public class RuntimePropertyElement_Field<TData, TBaseField> : RuntimePropertyElement_Field<TData> where TBaseField : BaseField<TData>, new()
    {
        public readonly TBaseField Field;
        public readonly IRuntimePropertyBinder Binding;
        
        public RuntimePropertyElement_Field([NotNull] IRuntimePropertyBinder binding) : base(binding)
        {
        }

        public override BaseField<TData> GetFieldView(IRuntimePropertyBinder binding)
        {
            var view = new TBaseField()
            {
                value = (TData) binding.Get(),
                label = binding.Label
            };
            return view;
        }
    }

    public class RuntimePropertyElement_String : RuntimePropertyElement_Field<string>
    {
        public RuntimePropertyElement_String(IRuntimePropertyBinder binding) : base(binding)
        {
        }
        
        public override BaseField<string> GetFieldView(IRuntimePropertyBinder binding)
        {
            if (binding == null)
            {
                return new TextField();
            }
            else
            {
                return new TextField()
                {
                    value = binding.Get() as string,
                    label = binding.Label
                };
            }
        }
    }

    public class RuntimePropertyElement_Missing : VisualElement
    {
        public readonly Label Label;

        public RuntimePropertyElement_Missing(string propertyName, System.Type dataType)
        {
            Label = new Label()
            {
                text = $"Unbound type {dataType} for property {propertyName}"
            };
            Add(Label);
        }
    }
}