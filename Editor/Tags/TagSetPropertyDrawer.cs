using System;
using System.Collections.Generic;
using System.Linq;
using NiftyFramework.Tags;
using NiftyFramework.UnityUtils;
using UnityEditor;
using UnityEngine;

namespace NiftyFramework.Core
{
    [CustomPropertyDrawer(typeof(TagSet))]
    public class TagSetPropertyDrawer : PropertyDrawer
    {
        private TagDatabase _tagDatabase;
        private TagSet m_CurrentTaggedSet;
        PopupList.InputData m_AssetLabels;
        private static int s_MaxShownLabels = 10;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var spData = property.FindPropertyRelative("_serializedData");
            //var spDatabase = property.FindPropertyRelative("_database");
            InitTagCache(spData);
            DrawLabelUI(position);
        }
        
        private void GetAllTags(SerializedProperty sp, out List<string> all, out List<string> partial)
        {
            int len = 0;
            
            all = new List<string>();
            partial = new List<string>();

            Dictionary<string, int> tagCount = new Dictionary<string, int>();

            if (sp.isArray)
            {
                while (sp.Next(false))
                {
                    string tag = sp.stringValue;
                    tagCount[tag] = tagCount.ContainsKey(tag) ? tagCount[tag] + 1 : 1;
                    len++;
                }
            }
            
            foreach (KeyValuePair<string,int> entry in tagCount)
            {
                var list = (entry.Value== tagCount.Count) ? all : partial;
                list.Add(entry.Key);
            }
        }
        
        private void GetAllTags(TagSet[] tagSet, out List<string> all, out List<string> partial)
        {
            all = new List<string>();
            partial = new List<string>();

            Dictionary<string, int> tagCount = new Dictionary<string, int>();
            foreach (TagSet tagSource in tagSet)
            {
                foreach (string tag in tagSource.Items)
                {
                    tagCount[tag] = tagCount.ContainsKey(tag) ? tagCount[tag] + 1 : 1;
                }
            }

            foreach (KeyValuePair<string, int> entry in tagCount)
            {
                var list = (entry.Value == tagSet.Length) ? all : partial;
                list.Add(entry.Key);
            }
        }
        
        public void InitTagCache(SerializedProperty sp)
        {
            GetAllTags(sp, out var all, out var partial);
                
                m_AssetLabels = new PopupList.InputData
                {
                    m_CloseOnSelection = false,
                    m_AllowCustom = true,
                    m_OnSelectCallback = HandleAssetSelected,
                    m_MaxCount = 15,
                    m_SortAlphabetically = true
                };

                TagDatabase database = GetDatabase();
                Dictionary<string, int> allTags = database.GetTagCount();
                foreach (var pair in allTags)
                {
                    PopupList.ListElement element = m_AssetLabels.NewOrMatchingElement(pair.Key);
                    if (element.filterScore < pair.Value)
                    {
                        element.filterScore = pair.Value;
                    }
                    element.selected = all.Any(label => string.Equals(label, pair.Key, StringComparison.OrdinalIgnoreCase));
                    element.partiallySelected = partial.Any(label => string.Equals(label, pair.Key, StringComparison.OrdinalIgnoreCase));
                }
                /*
                HashSet<ITagSource> newTaggedSet = new HashSet<ITagSource>();
                if (sp.isArray)
                {
                    while (!sp.NextVisible(false))
                    {
                        newTaggedSet.Add(sp.stringValue);
                    }
                }
                
                if (m_CurrentTagedSet == null || m_CurrentTagedSet.SetEquals(newTaggedSet))
                {
                    List<string> all;
                    List<string> partial;
                    GetAllTags(sources, out all, out partial);
                    
                    m_AssetLabels = new PopupList.InputData
                    {
                        m_CloseOnSelection = false,
                        m_AllowCustom = true,
                        m_OnSelectCallback = HandleAssetSelected,
                        m_MaxCount = 15,
                        m_SortAlphabetically = true
                    };
    
                    
                    Dictionary<string, float> allTags = TagDatabase.instance.GetAll();
                    foreach (var pair in allTags)
                    {
                        PopupList.ListElement element = m_AssetLabels.NewOrMatchingElement(pair.Key);
                        if (element.filterScore < pair.Value)
                        {
                            element.filterScore = pair.Value;
                        }
                        element.selected = all.Any(label => string.Equals(label, pair.Key, StringComparison.OrdinalIgnoreCase));
                        element.partiallySelected = partial.Any(label => string.Equals(label, pair.Key, StringComparison.OrdinalIgnoreCase));
                    }
                }*/
            /*
            // Init only if new asset
            if (m_CurrentAssetsSet == null || !m_CurrentAssetsSet.SetEquals(newAssetSet))
            {
                List<string> all;
                List<string> partial;
                GetAllTags(sources, out all, out partial);

                m_AssetLabels = new PopupList.InputData
                {
                    m_CloseOnSelection = false,
                    m_AllowCustom = true,
                    m_OnSelectCallback = HandleAssetSelected,
                    m_MaxCount = 15,
                    m_SortAlphabetically = true
                };

                Dictionary<string, float> allLabels = AssetDatabase.GetAllLabels();
                foreach (var pair in allLabels)
                {
                    PopupList.ListElement element = m_AssetLabels.NewOrMatchingElement(pair.Key);
                    if (element.filterScore < pair.Value)
                    {
                        element.filterScore = pair.Value;
                    }
                    element.selected = all.Any(label => string.Equals(label, pair.Key, StringComparison.OrdinalIgnoreCase));
                    element.partiallySelected = partial.Any(label => string.Equals(label, pair.Key, StringComparison.OrdinalIgnoreCase));
                }
            }
            m_CurrentAssetsSet = newAssetSet;
            m_CurrentChanged = false;*/
        }

        public TagDatabase GetDatabase(string path = "Assets/Config/TagDatabase.asset")
        {
            if (_tagDatabase != null)
            {
                return _tagDatabase;
            }
            var existingAssetGUID = AssetDatabase.FindAssets("t:TagDatabase");
            if (existingAssetGUID != null && existingAssetGUID.Length == 1)
            {
                string loadPath = AssetDatabase.GUIDToAssetPath(existingAssetGUID[0]);
                _tagDatabase = AssetDatabase.LoadAssetAtPath<TagDatabase>(loadPath);
            }
            else
            {
                _tagDatabase = ScriptableObject.CreateInstance<TagDatabase>();
                AssetDatabase.CreateAsset(_tagDatabase, path);
            }
            return _tagDatabase;
        }

        private void HandleAssetSelected(PopupList.ListElement element)
        {
            Debug.Log(element.text);
        }


        public void DrawLabelUI(Rect position)
        {
            

            // For the label list as a whole
            // The previous layouting means we've already lost a pixel to the left and couple at the top, so it is an attempt at horizontal padding: 3, verical padding: 5
            // (the rounded sides of labels makes this look like the horizontal and vertical padding is the same)
            float leftPadding = 1.0f;
            float rightPadding = 2.0f;
            float topPadding = 3.0f;
            float bottomPadding = 5.0f;

            GUIStyle labelButton = NiftyEditor.GetStyle("AssetLabel Icon");

            float buttonWidth = labelButton.margin.left + labelButton.fixedWidth + rightPadding;

            // Assumes we are already in a vertical layout
            // GUILayout.Space(topPadding);

            // Create a rect to test how wide the label list can be
            Rect widthProbeRect = position;
            widthProbeRect.width -= buttonWidth; // reserve some width for the button

            //EditorGUILayout.BeginHorizontal();

            
            // Left padding
            //GUILayoutUtility.GetRect(leftPadding, leftPadding, 0, 0);
            position.x += leftPadding;

            // Draw labels (fully selected)
            DrawLabelList(position,false);

            // Draw labels (partially selected)
            DrawLabelList(position,true);

           // GUILayout.FlexibleSpace();

            Rect r = position; //GUILayoutUtility.GetRect(labelButton.fixedWidth, labelButton.fixedWidth, labelButton.fixedHeight + bottomPadding, labelButton.fixedHeight + bottomPadding);
            r.x = widthProbeRect.xMax + labelButton.margin.left;
            if (EditorGUI.DropdownButton(r, GUIContent.none, FocusType.Passive, labelButton))
            {
                PopupWindow.Show(r, new PopupList(m_AssetLabels));
            }

            //EditorGUILayout.EndHorizontal();
        }
        
        private void DrawLabelList(Rect position, bool partiallySelected)
        {
            GUIContent[] test = new[] { new GUIContent("Test"), new GUIContent("Foo"), new GUIContent("Bar") };
            GUIStyle labelStyle = partiallySelected ? NiftyEditor.GetStyle("AssetLabel Partial") : NiftyEditor.GetStyle("AssetLabel");;
            Event evt = Event.current;
            //foreach (GUIContent content in (from i in m_AssetLabels.m_ListElements where (partiallySelected ? i.partiallySelected : i.selected) orderby i.text.ToLower() select i.m_Content).Take(s_MaxShownLabels))
            Rect rt = new Rect(position.x, position.y, 0,0);
            float xMax = position.width;
            foreach (GUIContent content in test)// where (partiallySelected ? i.partiallySelected : i.selected) orderby i.text.ToLower() select i.m_Content).Take(s_MaxShownLabels))
            {
                Vector2 uiSize =  labelStyle.CalcSize(content);
                rt.width = uiSize.x;
                rt.height = uiSize.y;

                //Rect rt = new Rect(0, 0, uiSize.x, uiSize.y);//EditorGUILayout.GetControlRect(false, content, labelStyle);
                if (Event.current.type == EventType.Repaint && rt.xMax >= xMax)
                    break;
                GUI.Label(rt, content, labelStyle);
                if (rt.xMax <= xMax && evt.type == EventType.MouseDown && rt.Contains(evt.mousePosition) && evt.button == 0 && GUI.enabled)
                {
                    evt.Use();
                    rt.x = xMax;
                    Debug.Log("Tag Clicked!");
                    PopupWindow.Show(rt, new PopupList(m_AssetLabels, content.text));
                }
                rt.x += rt.width + 2;
            }
        }

    }
}