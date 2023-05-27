using Background.SplinePath;
using UnityEditor;
using UnityEngine;

namespace Editor.Spline
{
    [CustomEditor(typeof(SplineCollider2D))]
    public class CustomEditorSplineCollider :UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            SplineCollider2D script = (SplineCollider2D)target;
            
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Generate Collider", GUILayout.MaxWidth(200)))
                {
                    script.CreateCollider();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
