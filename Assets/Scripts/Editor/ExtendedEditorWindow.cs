using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ExtendedEditorWindow : EditorWindow
    {
        protected SerializedObject serializedObject;
        protected SerializedProperty currentProperty;

        private string selectedPropertyPath;
        protected SerializedProperty selectedProperty;

        private Vector2 ScrollPosition = Vector2.zero;

        protected void DrawProperties(SerializedProperty property, bool drawChildren)
        {
            string lastPropPath = string.Empty;
            foreach (SerializedProperty p in property)
            {
                if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                    EditorGUILayout.EndHorizontal();

                    if (p.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperties(p, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }

                    lastPropPath = p.propertyPath;
                    EditorGUILayout.PropertyField(p, drawChildren);
                }
            }
        }

        protected void DrawSideBar(SerializedProperty property)
        {
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, "box");
            foreach (SerializedProperty p in property)
            {
                if (p == null)continue;
                if (GUILayout.Button(p.displayName)) selectedPropertyPath = p.propertyPath;
            }
            EditorGUILayout.EndScrollView();

            if (!string.IsNullOrEmpty(selectedPropertyPath)) selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
        }

        protected void DrawField(string propName, bool relative)
        {
            if (relative && currentProperty != null) EditorGUILayout.PropertyField(currentProperty.FindPropertyRelative(propName),true);
            else if(serializedObject != null) EditorGUILayout.PropertyField(serializedObject.FindProperty(propName), true);
        }
    }
}
