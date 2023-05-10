using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Bezie_Spline))]
    public class BezieSplineCustomEditor : GeneralSplineCustomEditor
    {
        private Bezie_Spline script;
        private SerializedProperty P_MirrorMode;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            script = (Bezie_Spline)target;
            P_MirrorMode = serializedObject.FindProperty(nameof(script.mirrorSplineArms));
        }
        protected override void LayoutForSpecificSpline()
        {
            script = (Bezie_Spline)target;
            GUILayout.BeginVertical(standartGUIStyle);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Segment")) { script.AddSegment(); }
            if (GUILayout.Button("Redraw")) { script.AssembleSpline(); }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            if (GUILayout.Button("Check For New Components")) { script.CheckForExistingComponents(); }
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(P_MirrorMode);
            if (script.mirrorSplineArms)
            { script.InitialMirrorArmPoints(); }
            GUILayout.EndVertical();
        }
    }
}