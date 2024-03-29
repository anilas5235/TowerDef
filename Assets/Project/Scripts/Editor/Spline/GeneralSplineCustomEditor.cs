using Background.SplinePath;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Spline
{
    public abstract class GeneralSplineCustomEditor : UnityEditor.Editor
    {
        protected SerializedProperty P_CurrentDrawMode,
            P_Tile,
            P_Resolution,
            P_LineColor,
            P_LineThickness,
            P_TileSizeMutliplier,
            P_Points,
            P_OffestAngle,
            P_CurrentPathSave,
            P_IgnoreNewComponents,
            P_Segments;

        protected GUIStyle standartGUIStyle;

        private BaseSplineBuilder script;

        private void Reset()
        {
            standartGUIStyle = new GUIStyle();
            standartGUIStyle.normal.background = MakeTex(600, 2, new Color(0, 0, 0, 0.2f));
            script = (BaseSplineBuilder)target;
            script.InitializeSpline();
        }

        protected virtual void OnEnable()
        {
            standartGUIStyle = new GUIStyle();
            standartGUIStyle.normal.background = MakeTex(600, 2, new Color(0, 0, 0, 0.2f));
            script = (BaseSplineBuilder)target;
            P_Tile = serializedObject.FindProperty(nameof(script.Tile));
            P_CurrentDrawMode = serializedObject.FindProperty(nameof(script.CurrentDrawMode));
            P_Resolution = serializedObject.FindProperty(nameof(script.RESOLUTION));
            P_LineColor = serializedObject.FindProperty(nameof(script.LineColor));
            P_LineThickness = serializedObject.FindProperty(nameof(script.LineThickness));
            P_TileSizeMutliplier = serializedObject.FindProperty(nameof(script.tileSizeMultiplier));
            P_Points = serializedObject.FindProperty(nameof(script.splinePoints));
            P_OffestAngle = serializedObject.FindProperty(nameof(script.offsetAngle));
            P_CurrentPathSave = serializedObject.FindProperty(nameof(script.CurrentPathPointSave));
            P_IgnoreNewComponents = serializedObject.FindProperty(nameof(script.ignoreNewComponents));
            P_Segments = serializedObject.FindProperty(nameof(script.DrawCurvesList));
        }

        public override void OnInspectorGUI()
        {
            script = (BaseSplineBuilder)target;

            LayoutForSpecificSpline();

            GUILayout.Space(10);

            EditorGUILayout.PropertyField(P_CurrentDrawMode);

            GUILayout.Space(5);

            EditorGUILayout.PropertyField(P_CurrentPathSave);
            if (GUILayout.Button("Save"))
            {
                script.CurrentPathPointSave.Points = script.GetAllUsedPoints();
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(script.CurrentPathPointSave);
                UnityEditor.AssetDatabase.SaveAssets();
#endif
            }

            EditorGUILayout.PropertyField(P_IgnoreNewComponents);
            
            GUILayout.Space(10);

            switch (script.CurrentDrawMode)
            {
                case BaseSplineBuilder.DrawMode.LineRender:
                    DrawModeLine();
                    break;
                case BaseSplineBuilder.DrawMode.ObjectTiling:
                    DrawModeTiling();
                    break;
            }
        
            EditorGUILayout.PropertyField(P_Points);
            EditorGUILayout.PropertyField(P_Segments);
        
            serializedObject.ApplyModifiedProperties();
        }

        protected abstract void LayoutForSpecificSpline();

        private void DrawModeLine()
        {
            EditorGUILayout.BeginVertical(standartGUIStyle);

            EditorGUILayout.PropertyField(P_LineColor);
            EditorGUILayout.PropertyField(P_Resolution);
            EditorGUILayout.PropertyField(P_LineThickness);

            EditorGUILayout.EndVertical();
        }

        private void DrawModeTiling()
        {
            EditorGUILayout.BeginVertical(standartGUIStyle);
            if (GUILayout.Button("Reset All Objects")) { script.DeleteAllTileObjects(); }
            if (GUILayout.Button("Delete Unseen Objects")) { script.DeleteAllTileObjects(); }
            EditorGUILayout.PropertyField(P_Tile);
            EditorGUILayout.PropertyField(P_OffestAngle);
            EditorGUILayout.PropertyField(P_Resolution);
            EditorGUILayout.PropertyField(P_TileSizeMutliplier);
            EditorGUILayout.EndVertical();
        }


        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}


