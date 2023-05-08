using Background.SplinePath;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(B_Spline))]
    public class SplineCustomEditor : UnityEditor.Editor
    {
        private void Reset()
        {
            B_Spline script = (B_Spline) target;
            script.InitializeSpline();
        }
        public override void OnInspectorGUI()
        {
            
            B_Spline script = (B_Spline) target;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Point")) { script.AddPointToSpline(); }
            if (GUILayout.Button("Redraw")) { script.AssembleSpline(); }
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button("Check For New Components")) { script.CheckForExistingComponents(); }
            
            base.OnInspectorGUI();
        }
    }
}

