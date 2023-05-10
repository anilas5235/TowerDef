using Background.SplinePath;
using UnityEditor;
using UnityEngine;

public abstract class GeneralSplineCustomEditor : UnityEditor.Editor
{
    protected SerializedProperty P_CurrentDrawMode,
        P_Tile,
        P_Resolution,
        P_LineColor,
        P_LineThickness,
        P_TileSizeMutliplier;

    protected GUIStyle standartGUIStyle = new GUIStyle();

    private BaseSplineBuilder script;

    private void Reset()
    {
        standartGUIStyle.normal.background = MakeTex(600, 2, new Color(1, 1, 1, 0.3f));
        script = (BaseSplineBuilder)target;
        script.InitializeSpline();
    }

    protected virtual void OnEnable()
    {
        standartGUIStyle.normal.background = MakeTex(600, 2, new Color(1, 1, 1, 0.3f));
        script = (BaseSplineBuilder)target;
        P_Tile = serializedObject.FindProperty(nameof(script.Tile));
        P_CurrentDrawMode = serializedObject.FindProperty(nameof(script.CurrentDrawMode));
        P_Resolution = serializedObject.FindProperty(nameof(script.RESOLUTION));
        P_LineColor = serializedObject.FindProperty(nameof(script.LineColor));
        P_LineThickness = serializedObject.FindProperty(nameof(script.LineThickness));
        P_TileSizeMutliplier = serializedObject.FindProperty(nameof(script.tileSizeMultiplier));
    }

    public override void OnInspectorGUI()
    {

        script = (BaseSplineBuilder)target;

        LayoutForSpecificSpline();

        GUILayout.Space(10);

        EditorGUILayout.PropertyField(P_CurrentDrawMode);

        GUILayout.Space(5);

        switch (script.CurrentDrawMode)
        {
            case BaseSplineBuilder.DrawMode.LineRender:
                DrawModeLine();
                break;
            case BaseSplineBuilder.DrawMode.ObjectTiling:
                DrawModeTiling();
                break;
        }

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
        EditorGUILayout.PropertyField(P_Tile);
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


