using System;
using Background.SplinePath;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Spline))]
    public class SplineCustomEditor : UnityEditor.Editor
    {
        private void Reset()
        {
            Spline script = (Spline) target;
            script.LoadPrefaps();
        }

        public override void OnInspectorGUI()
        {
            
            Spline script = (Spline) target;
            script.usedSplineType = (Spline.SplineType) EditorGUILayout.EnumPopup("SplineType", script.usedSplineType);
            script.SplineTypeChange();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Draw Spline"))
            {
                script.AssembleSpline();
            }
        
            if (GUILayout.Button("Add Point"))
            {
                script.AddPoint();
            }
        
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Load Prefabs"))
            {
                script.LoadPrefaps();
            }
            base.OnInspectorGUI();
        }
    }
}

