using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Bezie_Spline))]
    public class BezieSplineCustomEditor : UnityEditor.Editor
    {
        private void Reset()
        {
            Bezie_Spline script = (Bezie_Spline) target;
            script.InitializeSpline();
        }
        public override void OnInspectorGUI()
        {
            Bezie_Spline script = (Bezie_Spline) target;
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Segment")) { script.AddSegment(); }
            if (GUILayout.Button("Redraw")) { script.AssembleSpline(); }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            if (GUILayout.Button("Check For New Components")) { script.CheckForExistingComponents(); }
            GUILayout.Space(5);
            script.mirrorSplineArms = EditorGUILayout.Toggle("mirror Arms", script.mirrorSplineArms);
            if (script.mirrorSplineArms)
            { script.InitialMirrorArmPoints(); }
            
            GUILayout.Space(20);

            base.OnInspectorGUI();
        }
    }
}