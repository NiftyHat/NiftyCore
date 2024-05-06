using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RuntimeProperties
{
    public class RuntimeBindingSampleWindow : EditorWindow
    {
        [MenuItem("Examples/RuntimeBindingSampleWindow")]
        public static void ShowExample()
        {
            RuntimeBindingSampleWindow wnd = GetWindow<RuntimeBindingSampleWindow>();
            wnd.titleContent = new GUIContent("MyEditorWindow");
        }

        public class ExampleDataA
        {
            public string String = "foo";
            public bool Boolean = true;
            public int Int = 0;
            public uint UnsignedInt =  0;
            public long Long = 0;
            public ulong UnsignedLong = 0;
            public double Double = 0;
            public string[] StringArray;
        }

        public ExampleDataA _testA = new ExampleDataA() { };
        public RuntimeObjectEditor _objectEditor;
        
        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // VisualElements objects can contain other VisualElement following a tree hierarchy
            Label label = new Label("Test");
            root.Add(label);

            // Create button
            _objectEditor = new RuntimeObjectEditor();
            root.Add(_objectEditor);

            root.Add(new Button(SetExampleA)
            {
                text = "Example A"
            }); 
            root.Add( new Button(SetNull)
            {
                text = "Clear"
            });
        }

        public void SetNull()
        {
            _objectEditor.Clear();
        }

        public void SetExampleA()
        {
            _objectEditor.Set(_testA, output =>
            {
                _testA = output as ExampleDataA;
            });
        }
    }
}