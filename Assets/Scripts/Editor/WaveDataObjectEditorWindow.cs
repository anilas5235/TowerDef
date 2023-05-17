using System;
using System.Collections.Generic;
using Background.WaveManaging;
using Scrips.Background;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class WaveDataObjectEditorWindow : ExtendedEditorWindow
    {
        private WavesData myWavesData;
        private Vector2 WaveScrollPosition = Vector2.zero, PatternToolBoxScrollPosition = Vector2.zero;
        private PatternBase.PatternTypes selectedPatternType = PatternBase.PatternTypes.Linear;
        private int MinValue, MaxValue, StartValue, LowerBound, UpperBound, linearValue;
        private int IDofSelectedWave;//Pattern Input vars
        

        public static void Open(WavesData dataObject)
        {
            WaveDataObjectEditorWindow window = GetWindow<WaveDataObjectEditorWindow>("Waves Data Editor");
            window.serializedObject = new SerializedObject(dataObject);
            window.minSize = new Vector2(800, 200);
        }

        private void OnGUI()
        {
            DrawHowlWindow();
        }

        private void DrawHowlWindow()
        {
            myWavesData = (WavesData)serializedObject.targetObject;

            currentProperty = serializedObject.FindProperty(nameof(myWavesData.Waves));
            
            DrawToolBar();

            EditorGUILayout.BeginHorizontal();
            
            DrawPatternToolBox();

            DrawWaveSectionBar();

            DrawWaveEditingBox();
            
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void AddWaveObject()
        {
            myWavesData.Waves.Add(new Wave());
            myWavesData.NameWaves();
            SaveChanges();
            serializedObject = new SerializedObject(serializedObject.targetObject);
            Repaint();
        }

        private void RemoveWaveObject(int index)
        {
            myWavesData.Waves.RemoveAt(index);
            myWavesData.NameWaves();
            selectedProperty = null;
            SaveChanges();
            serializedObject = new SerializedObject(serializedObject.targetObject);
            Repaint();
        }

        private void DrawSelectedPropertiesPanel()
        {
            currentProperty = selectedProperty;
            IDofSelectedWave = currentProperty.FindPropertyRelative("ID").intValue;
            WaveScrollPosition = EditorGUILayout.BeginScrollView(WaveScrollPosition, "box");
            
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField(currentProperty.displayName,EditorStyles.boldLabel,GUILayout.MaxWidth(70));
            int currentCount = myWavesData.Waves[IDofSelectedWave].SpawnData.Count;
            int newCount = currentCount;
             newCount   = InputFieldWithLabel(newCount, "Size",50);
            if (GUILayout.Button("Apply",GUILayout.MaxWidth(50)))
            {
                if (newCount != currentCount)
                {
                    List<WavePoint> oldSpawnData = myWavesData.Waves[IDofSelectedWave].SpawnData;
                    List<WavePoint> newSpawnData = new List<WavePoint>();
                    for (int i = 0; i < newCount; i++)
                    {
                        newSpawnData.Add(i < currentCount - 1 ? oldSpawnData[i] : new WavePoint());
                    }
                    myWavesData.Waves[IDofSelectedWave].SpawnData = newSpawnData;
                }
            }
            EditorGUILayout.EndHorizontal();
            
            myWavesData.Waves[currentProperty.FindPropertyRelative("ID").intValue].NameSteps();

            serializedObject.ApplyModifiedProperties();
            SaveChanges();
            serializedObject = new SerializedObject(serializedObject.targetObject);

            SerializedProperty thisSpawnData = currentProperty.FindPropertyRelative("SpawnData");
            EditorGUILayout.BeginVertical();
            DrawProperties(thisSpawnData,true);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        private void DrawToolBar()
        {
            EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("Tool Bar",EditorStyles.boldLabel,GUILayout.MaxWidth(155));
            if (GUILayout.Button("Add Wave",GUILayout.Height(30f),GUILayout.MaxWidth(100))) AddWaveObject();
            if (selectedProperty != null)
            {
                if (GUILayout.Button("Delete Wave",GUILayout.Height(30f),GUILayout.MaxWidth(100))) RemoveWaveObject(selectedProperty.FindPropertyRelative("ID").intValue);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawWaveSectionBar()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
            EditorGUILayout.LabelField("Wave Selection",EditorStyles.boldLabel,GUILayout.MaxWidth(100));
            WavePointCustomPropertyDrawer.PP_BackgroundIndicator = 0;
            DrawSideBar(currentProperty);
            EditorGUILayout.EndVertical();
        }

        private void DrawWaveEditingBox()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true),GUILayout.MinWidth(150+(18+4)*15));
            if (selectedProperty != null) DrawSelectedPropertiesPanel();
            else EditorGUILayout.LabelField($"Select one of the Wave to Edit",EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();
        }

        private void DrawPatternToolBox()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
            PatternToolBoxScrollPosition = EditorGUILayout.BeginScrollView(PatternToolBoxScrollPosition, "box");
            
            EditorGUILayout.LabelField("Pattern Tool",EditorStyles.boldLabel,GUILayout.MaxWidth(100));
            selectedPatternType = (PatternBase.PatternTypes)EditorGUILayout.EnumPopup(selectedPatternType);

            int StandartLabelSize = 100;
            
            LowerBound = InputFieldWithLabel(LowerBound, "LowerBound",StandartLabelSize);
            UpperBound = InputFieldWithLabel(UpperBound, "UpperBound",StandartLabelSize);

            switch (selectedPatternType)
            {
                case PatternBase.PatternTypes.DiagonalDescending:
                    StartValue = InputFieldWithLabel(StartValue, $"StartValue",StandartLabelSize);
                    break;
                case PatternBase.PatternTypes.DiagonalRising:
                    break;
                case PatternBase.PatternTypes.Linear:
                    linearValue = InputFieldWithLabel(linearValue, "Value",StandartLabelSize);
                    break;
                case PatternBase.PatternTypes.ZigZack:
                    break;
                case PatternBase.PatternTypes.Custom:
                    break;
                case PatternBase.PatternTypes.Random:
                    MinValue = InputFieldWithLabel(MinValue, $"MinValue",StandartLabelSize);
                    MaxValue = InputFieldWithLabel(MaxValue, $"MaxValue",StandartLabelSize);
                    break;
                default: return;
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Apply Pattern",GUILayout.Height(25)))
            {
                PatternBase pattern = null;
                switch (selectedPatternType)
                {
                    case PatternBase.PatternTypes.DiagonalDescending:
                        break;
                    case PatternBase.PatternTypes.DiagonalRising:
                        break;
                    case PatternBase.PatternTypes.Linear:
                        pattern = new LinearPattern(linearValue);
                        break;
                    case PatternBase.PatternTypes.ZigZack:
                        break;
                    case PatternBase.PatternTypes.Custom:
                        break;
                    case PatternBase.PatternTypes.Random:
                        break;
                    default: return;
                }

                if (pattern != null && selectedProperty != null) ApplyPattern( pattern, LowerBound, UpperBound);
                else Debug.Log("Pattern could not be applied");
            }
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void ApplyPattern( PatternBase pattern, int lowerBoundIndex, int upperBoundIndex)
        {
            if (lowerBoundIndex < 0) lowerBoundIndex = 0;
            int aryLength = myWavesData.Waves[selectedProperty.FindPropertyRelative("ID").intValue].SpawnData.Count;
            if (upperBoundIndex > aryLength) upperBoundIndex = aryLength;
            int waveID = selectedProperty.FindPropertyRelative("ID").intValue;
            for (int i = lowerBoundIndex; i < upperBoundIndex; i++)
            {
                myWavesData.Waves[waveID].SpawnData[i].EnemyData[pattern.GetValue()] = 1;
            }
            serializedObject = new SerializedObject(serializedObject.targetObject);
        }

        private static int InputFieldWithLabel(int value,string labelName,int labelSize)
        {
            EditorGUILayout.LabelField(labelName+":", GUILayout.MaxWidth(labelSize));
            if (Int32.TryParse(EditorGUILayout.TextArea($"{value}",GUILayout.MaxWidth(70)),out var newVal))
            {
                value = newVal;
            }
            return value;
        }
    }
}
