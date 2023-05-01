using System;
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
            script.LoadPrefabs();
            script.CheckForExistingComponents();
        }
        public override void OnInspectorGUI()
        {
            
            B_Spline script = (B_Spline) target;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Draw Spline"))
            {
                script.AssembleSpline();
            }
        
            if (GUILayout.Button("Add Point"))
            {
                script.AddPointToSpline();
            }
        
            GUILayout.EndHorizontal();
            //if (GUILayout.Button("Load Prefabs")) { script.LoadPrefabs(); }
            base.OnInspectorGUI();
        }
    }
}

