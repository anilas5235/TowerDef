using System;
using Background.WaveManaging;
using Scrips.Background;
using Scrips.Background.WaveManaging;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class WaveDataObjectEditorWindow : ExtendedEditorWindow
    {
        private WavesData myWavesData;
        
        private Vector2 WaveScrollPosition = Vector2.zero, ToolBarScrollPosition = Vector2.zero;
        private static GUIStyle guiStyleLight = new GUIStyle(), guiStyleDark = new GUIStyle();


        public static void Open(WavesData dataObject)
        {
            WaveDataObjectEditorWindow window = GetWindow<WaveDataObjectEditorWindow>("Waves Data Editor");
            window.serializedObject = new SerializedObject(dataObject);
            window.minSize = new Vector2(800, 200);
            guiStyleLight.normal.background = MakeTexture2D(50, 2, new Color(1, 1, 1, 0.3f));
            guiStyleDark.normal.background = MakeTexture2D(50, 2, new Color(1, 1, 1, 0.1f));
        }

        private void OnGUI()
        {
            myWavesData = (WavesData)serializedObject.targetObject;

            currentProperty = serializedObject.FindProperty(nameof(myWavesData.Waves));

            EditorGUILayout.BeginHorizontal();
            
            DrawToolBar();
            
            DrawWaveSectionBar();

            DrawWaveEditingBox();
            
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
            serializedObject = new SerializedObject(serializedObject.targetObject);
        }

        private void AddWaveObject()
        {
            myWavesData.Waves.Add(new Wave());
            myWavesData.NameWaves();
            SaveChanges();
            serializedObject = new SerializedObject(serializedObject.targetObject);
        }

        private void RemoveWaveObject(int index)
        {
            myWavesData.Waves.RemoveAt(index);
            myWavesData.NameWaves();
            SaveChanges();
            serializedObject = new SerializedObject(serializedObject.targetObject);
        }

        private void DrawSelectedPropertiesPanel()
        {
            currentProperty = selectedProperty;
            WaveScrollPosition = EditorGUILayout.BeginScrollView(WaveScrollPosition, "box");
            
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField(currentProperty.displayName,EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            
            myWavesData.Waves[currentProperty.FindPropertyRelative("ID").intValue].NameSteps();

            serializedObject.ApplyModifiedProperties();
            SaveChanges();
            serializedObject = new SerializedObject(serializedObject.targetObject);

            SerializedProperty thisSpawnData = currentProperty.FindPropertyRelative("SpawnData");
            /*
            EditorGUILayout.BeginFoldoutHeaderGroup(true, "WaveData");
            for (int i = 0; i < thisSpawnData.arraySize; i++)
            {
                GUIStyle guiStyleForNext = i % 2 == 0 ? guiStyleLight : guiStyleDark;
                EditorGUILayout.BeginHorizontal(guiStyleForNext);
                EditorGUILayout.LabelField($"Step{i}",GUILayout.MinWidth(100));
                EditorGUILayout.PropertyField(thisSpawnData.GetArrayElementAtIndex(i),true);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            */

            EditorGUILayout.PropertyField(thisSpawnData,true);
            /*
            if (GUILayout.Button("+"))
            {
                myWavesData.Waves[currentProperty.FindPropertyRelative("ID").intValue].SpawnData.Add(new WavePoint());
                serializedObject = new SerializedObject(serializedObject.targetObject);
            }
            */
            EditorGUILayout.EndScrollView();
        }

        private void DrawToolBar()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.MinWidth(150), GUILayout.ExpandHeight(true));
            ToolBarScrollPosition = EditorGUILayout.BeginScrollView(ToolBarScrollPosition, "box");
            if (GUILayout.Button("Add Wave")) AddWaveObject();
            if (selectedProperty != null)
            {
                if (GUILayout.Button("Delete Wave")) RemoveWaveObject(selectedProperty.FindPropertyRelative("ID").intValue);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawWaveSectionBar()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.MinWidth(150), GUILayout.ExpandHeight(true));
            DrawSideBar(currentProperty);
            EditorGUILayout.EndVertical();
        }

        private void DrawWaveEditingBox()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
            if (selectedProperty != null) DrawSelectedPropertiesPanel();
            else EditorGUILayout.LabelField($"Select one of the Wave to Edit",EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();
        }
        
        private static Texture2D MakeTexture2D(int width, int height, Color col)
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
