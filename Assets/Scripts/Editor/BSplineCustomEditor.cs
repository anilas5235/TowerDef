using Background.SplinePath;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(B_Spline))]
    public class BSplineCustomEditor : GeneralSplineCustomEditor
    {
        private SerializedProperty P_TentionScale;
        private B_Spline script;

        protected override void OnEnable()
        {
            base.OnEnable();
            script = (B_Spline)target;
            P_TentionScale = serializedObject.FindProperty(nameof(script.TentionScale));
        }

        protected override void LayoutForSpecificSpline()
        {
            script = (B_Spline)target;
            GUILayout.BeginVertical(standartGUIStyle);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Point")) script.AddPointToSpline();
            if (GUILayout.Button("Redraw")) script.AssembleSpline();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            if (GUILayout.Button("Check For New Components")) script.CheckForExistingComponents();
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(P_TentionScale);
            GUILayout.EndVertical();
        }
    }
}

