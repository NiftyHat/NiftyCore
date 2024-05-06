using UnityEngine.UIElements;

namespace RuntimeProperties
{
    public class RuntimeObjectEditor : VisualElement
    {
        private readonly Label LabelName;
        private HelpBox InfoBox;
        private Button AssignButton;
        private object _data;
        private System.Action<object> _assign;
        public readonly RuntimePropertyListElement PropertyList;
        
        public RuntimeObjectEditor()
        {
            LabelName = new Label("Runtime Object Editor");
            InfoBox = new HelpBox("Runtime Object editor needs to be manually sent data by invoking Set()",
                HelpBoxMessageType.Info);
            AssignButton = new Button(HandleClickAssign);
            PropertyList = new RuntimePropertyListElement();
            Add(LabelName);
            Add(InfoBox);
            Add(PropertyList);
        }

        private void HandleClickAssign()
        {
            _assign?.Invoke(_data);
        }

        public void Set(object data, System.Action<object> assign = null)
        {
            _data = data;
            InfoBox.style.display = DisplayStyle.None;
            _assign = assign;
            AssignButton.SetEnabled(assign != null);
            PropertyList.Set(data);
        }

        public void Clear()
        {
            _data = null;
            InfoBox.style.display = DisplayStyle.Flex;
            _assign = null;
            AssignButton.SetEnabled(false);
        }
    }
}