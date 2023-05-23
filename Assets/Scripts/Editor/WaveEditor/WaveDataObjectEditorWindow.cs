using System;
using System.Collections.Generic;
using System.Linq;
using Background.WaveManaging;
using Editor.Others;
using Editor.WaveEditor.WavePattern;
using UnityEditor;
using UnityEngine;

namespace Editor.WaveEditor
{
    public class WaveDataObjectEditorWindow : ExtendedEditorWindow
    {
        private WavesData myWavesData;
        private Vector2 WaveScrollPosition = Vector2.zero, PatternToolBoxScrollPosition = Vector2.zero;
        private PatternBase.PatternTypes selectedPatternType = PatternBase.PatternTypes.Linear;
        private int MinValue, MaxValue, StartValue, StepLowerBound, StepUpperBound, linearValue,ValueLowerBound,ValueUpperBound;//Pattern Input vars
        private int InputValueWaveSize, SizeOfCustomPattern = 5;
        private int[] CustomPattern;
        private int IdOfSelectedWave = -1;
        private int StandartLabelSize = 70, StandartFieldSize = 70;
        private WavePoint[] CopiedWaveData;
        private CustomPatternSave loadedCustomPatternSave;
        private static GUIContent emptyLabel;

        public static void Open(WavesData dataObject)
        {
            WaveDataObjectEditorWindow window = GetWindow<WaveDataObjectEditorWindow>("Waves Data Editor");
            window.serializedObject = new SerializedObject(dataObject);
            window.minSize = new Vector2(1000, 200);
            EditorUtility.SetDirty(dataObject);
        }

        private void OnEnable()
        {
            CustomPattern = new int[SizeOfCustomPattern];
            emptyLabel = GUIContent.none;
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
            myWavesData.Waves ??= new List<Wave>();
            Wave newWave = new Wave
            {
                SpawnData = new List<WavePoint>()
            };
            myWavesData.Waves.Add(newWave);
            myWavesData.NameWaves();
            SaveChanges();
            serializedObject = new SerializedObject(serializedObject.targetObject);
        }

        private void RemoveWaveObject(int index)
        {
            myWavesData.Waves.RemoveAt(index);
            myWavesData.NameWaves();
            selectedProperty = null;
            SaveChanges();
            serializedObject = new SerializedObject(serializedObject.targetObject);
        }

        private void DrawSelectedPropertiesPanel()
        {

            currentProperty = selectedProperty;
            if (IdOfSelectedWave != currentProperty.FindPropertyRelative("ID").intValue)
            {
                IdOfSelectedWave = currentProperty.FindPropertyRelative("ID").intValue;
                WaveScrollPosition = Vector2.zero;
                InputValueWaveSize = myWavesData.Waves[IdOfSelectedWave].SpawnData.Count;
            }

            WaveScrollPosition = EditorGUILayout.BeginScrollView(WaveScrollPosition, "box");
            {
                EditorGUILayout.BeginVertical();
                {

                    EditorGUILayout.BeginHorizontal("box");
                    {
                        EditorGUILayout.LabelField(currentProperty.displayName, EditorStyles.boldLabel,
                            GUILayout.MaxWidth(70));

                        InputValueWaveSize =
                            EditorCustomFunctions.IntInputFieldWithLabel(InputValueWaveSize, "Size", 50,
                                StandartFieldSize);
                        if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                            OverrideWaveDataLength(IdOfSelectedWave, InputValueWaveSize);
                    }
                    EditorGUILayout.EndHorizontal();

                    SerializedProperty thisSpawnData = currentProperty.FindPropertyRelative("SpawnData");

                    EditorGUILayout.BeginVertical();
                    {
                        Rect rect = new Rect(0, 30, WavePoint.WavePointSize * 20, 20);
                        for (int i = 0; i < thisSpawnData.arraySize; i++){

                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUI.PropertyField(rect,thisSpawnData.GetArrayElementAtIndex(i), emptyLabel);

                                myWavesData.Waves[IdOfSelectedWave].SpawnData[i].ExtraWait =
                                    EditorCustomFunctions.IntInputFieldWithLabel(
                                        myWavesData.Waves[IdOfSelectedWave].SpawnData[i].ExtraWait, "+Time",
                                        50, 30, 20);
                            }
                            EditorGUILayout.EndHorizontal();
                            rect.x += rect.height+2;
                            EditorGUILayout.Space(2);
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginHorizontal();
                    {

                        if (GUILayout.Button("+", GUILayout.MaxWidth(StandartLabelSize / 2f), GUILayout.Height(20)))
                        {
                            myWavesData.Waves[IdOfSelectedWave].SpawnData.Add(new WavePoint());
                            myWavesData.Waves[IdOfSelectedWave].NameSteps();
                            serializedObject.ApplyModifiedProperties();
                            SaveChanges();
                            serializedObject = new SerializedObject(serializedObject.targetObject);
                            InputValueWaveSize = myWavesData.Waves[IdOfSelectedWave].SpawnData.Count;

                        }

                        if (GUILayout.Button("-", GUILayout.MaxWidth(StandartLabelSize / 2f), GUILayout.Height(20)))
                        {
                            myWavesData.Waves[IdOfSelectedWave].SpawnData
                                .RemoveAt(myWavesData.Waves[IdOfSelectedWave].SpawnData.Count - 1);
                            SaveChanges();
                            serializedObject = new SerializedObject(serializedObject.targetObject);
                            InputValueWaveSize = myWavesData.Waves[IdOfSelectedWave].SpawnData.Count;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                } EditorGUILayout.EndVertical();
            }
                
            EditorGUILayout.EndScrollView();
        }

        private void OverrideWaveDataLength(int waveID,int newLength)
        {
            int sizeOfOriginalSpawnData = myWavesData.Waves[waveID].SpawnData.Count;
            if (newLength != sizeOfOriginalSpawnData)
            {
                if (waveID == IdOfSelectedWave) InputValueWaveSize = newLength;
                
                List<WavePoint> oldSpawnData = myWavesData.Waves[waveID].SpawnData;
                List<WavePoint> newSpawnData = new List<WavePoint>();
                for (int i = 0; i < newLength; i++)
                    newSpawnData.Add(i < sizeOfOriginalSpawnData - 1 ? oldSpawnData[i] : new WavePoint());

                myWavesData.Waves[waveID].SpawnData = newSpawnData;
                if (newLength > sizeOfOriginalSpawnData) myWavesData.Waves[waveID].NameSteps();
                serializedObject.ApplyModifiedProperties();
                SaveChanges();
                serializedObject = new SerializedObject(serializedObject.targetObject);
            }
        }

        private void DrawToolBar()
        {
            EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
            {
                int sizeOfToolBarButtons = 115;
                EditorGUILayout.LabelField("Tools:", EditorStyles.boldLabel, GUILayout.MaxWidth(55));
                if (GUILayout.Button("Add Wave", GUILayout.Height(30f), GUILayout.MaxWidth(sizeOfToolBarButtons))) AddWaveObject();
                if (selectedProperty != null)
                {
                    if (IdOfSelectedWave < myWavesData.Waves.Count)
                    {
                        if (GUILayout.Button("Delete Wave", GUILayout.Height(30f),
                                GUILayout.MaxWidth(sizeOfToolBarButtons)))
                        {RemoveWaveObject(IdOfSelectedWave); return;}
                        if (GUILayout.Button("Clear Wave Data", GUILayout.Height(30f),
                                GUILayout.MaxWidth(sizeOfToolBarButtons)))
                            ClearDataOfWave(IdOfSelectedWave);
                        if (GUILayout.Button("Copy Wave Data", GUILayout.Height(30f),
                                GUILayout.MaxWidth(sizeOfToolBarButtons)))
                            CopyWaveDataOfWave(IdOfSelectedWave);
                        if (CopiedWaveData != null)
                        {
                            if (GUILayout.Button("Past Wave Data", GUILayout.Height(30f),
                                    GUILayout.MaxWidth(sizeOfToolBarButtons)))
                                PasteWaveDataToWave(IdOfSelectedWave);
                        }

                        if (myWavesData.Waves[IdOfSelectedWave].SpawnData.Count > 0)
                        {
                            if (GUILayout.Button("Delete Wave Data", GUILayout.Height(30f),
                                    GUILayout.MaxWidth(sizeOfToolBarButtons)))
                                OverrideWaveDataLength(IdOfSelectedWave, 0);
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ClearDataOfWave(int waveID)
        {
            int size = myWavesData.Waves[IdOfSelectedWave].SpawnData.Count;
            List<WavePoint> newSpawnData = new List<WavePoint>();
            for (int i = 0; i < size; i++)
            {
                newSpawnData.Add(new WavePoint());
            }
            myWavesData.Waves[waveID].SpawnData = newSpawnData;
            myWavesData.Waves[waveID].NameSteps();
            serializedObject = new SerializedObject(serializedObject.targetObject);
        }

        private void CopyWaveDataOfWave(int waveID)
        {
            CopiedWaveData = myWavesData.Waves[waveID].SpawnData.ToArray();
            serializedObject = new SerializedObject(serializedObject.targetObject);
        }

        private void PasteWaveDataToWave(int waveID)
        {
            myWavesData.Waves[waveID].SpawnData = CopiedWaveData.ToList();
            if (waveID == IdOfSelectedWave)
                InputValueWaveSize = myWavesData.Waves[IdOfSelectedWave].SpawnData.Count;
            myWavesData.Waves[waveID].NameSteps();
            serializedObject = new SerializedObject(serializedObject.targetObject);
        }

        private void DrawWaveSectionBar()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
            {
                EditorGUILayout.LabelField("Wave Selection", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
                WavePointCustomPropertyDrawer.PP_BackgroundIndicator = 0;
                DrawSideBar(currentProperty);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawWaveEditingBox()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(true));
            {
                if (selectedProperty != null) DrawSelectedPropertiesPanel();
                else EditorGUILayout.LabelField($"Select one of the Wave to Edit", EditorStyles.boldLabel);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawPatternToolBox()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
            {
                PatternToolBoxScrollPosition = EditorGUILayout.BeginScrollView(PatternToolBoxScrollPosition, "box");
                {
                    EditorGUILayout.LabelField("Pattern Tool", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
                    selectedPatternType = (PatternBase.PatternTypes)EditorGUILayout.EnumPopup(selectedPatternType);

                    if (selectedPatternType == PatternBase.PatternTypes.Custom)
                    {
                        loadedCustomPatternSave =
                            (CustomPatternSave)EditorGUILayout.ObjectField(loadedCustomPatternSave,
                                typeof(CustomPatternSave));
                        EditorGUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("Load", GUILayout.MaxWidth(40)))
                            {
                                if (loadedCustomPatternSave != null)
                                {
                                    CustomPattern = loadedCustomPatternSave.customPattern;
                                    SizeOfCustomPattern = CustomPattern.Length;
                                }
                            }

                            if (GUILayout.Button("Save To Obj", GUILayout.MaxWidth(90)))
                            {
                                if (loadedCustomPatternSave != null)
                                {
                                    loadedCustomPatternSave.customPattern = CustomPattern;
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    GUILayout.Space(10);

                    StepLowerBound = EditorCustomFunctions.IntInputFieldWithLabel(StepLowerBound, "From Step", StandartLabelSize,StandartFieldSize);
                    StepUpperBound = EditorCustomFunctions.IntInputFieldWithLabel(StepUpperBound, "To Step", StandartLabelSize,StandartFieldSize);

                    switch (selectedPatternType)
                    {
                        case PatternBase.PatternTypes.DiagonalDescending:
                            StandardInput();
                            break;
                        case PatternBase.PatternTypes.DiagonalRising:
                            StandardInput();
                            break;
                        case PatternBase.PatternTypes.Linear:
                            linearValue = EditorCustomFunctions.IntInputFieldWithLabel(linearValue, "Value", StandartLabelSize,StandartFieldSize);
                            break;
                        case PatternBase.PatternTypes.ZigZack:
                            StandardInput();
                            break;
                        case PatternBase.PatternTypes.Custom:
                          
                            SizeOfCustomPattern = EditorCustomFunctions.IntInputFieldWithLabel(SizeOfCustomPattern, "Size", StandartLabelSize,StandartFieldSize);

                            if (GUILayout.Button("Apply Size", GUILayout.MaxWidth(100),GUILayout.MinWidth(40)))
                            {
                                if (CustomPattern.Length != SizeOfCustomPattern)
                                {
                                    int[] oldCustomPattern = CustomPattern;
                                    CustomPattern = new int[SizeOfCustomPattern];
                                    for (int i = 0; i < CustomPattern.Length; i++)
                                    {
                                        if (i < oldCustomPattern.Length) CustomPattern[i] = oldCustomPattern[i];
                                    }
                                }
                            }
                            
                            
                            GUILayout.Space(10);

                            for (int i = 0; i < Math.Ceiling(CustomPattern.Length / 5f); i++)
                            {

                                EditorGUILayout.BeginHorizontal(
                                    i % 2 == 0
                                        ? EditorCustomFunctions.GetStandardGUIStyle(EditorCustomFunctions
                                            .StandardGUIStyles.DarkGray)
                                        : EditorCustomFunctions.GetStandardGUIStyle(EditorCustomFunctions
                                            .StandardGUIStyles.LightGray), GUILayout.MaxWidth(140));
                                {
                                    for (int j = 0; j < 5; j++)
                                    {
                                        if (i * 5 + j < CustomPattern.Length)
                                        {
                                            int value = CustomPattern[i * 5 + j];
                                            if (Int32.TryParse(EditorGUILayout.TextArea($"{value}",
                                                    GUILayout.MaxWidth(23)), out var newVal))
                                            {
                                                value = newVal;
                                            }

                                            CustomPattern[i * 5 + j] = value;
                                        }
                                        else break;
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }

                            break;
                        case PatternBase.PatternTypes.Random:
                            MinValue = EditorCustomFunctions.IntInputFieldWithLabel(MinValue, $"MinValue", StandartLabelSize,StandartFieldSize);
                            MaxValue = EditorCustomFunctions.IntInputFieldWithLabel(MaxValue, $"MaxValue", StandartLabelSize,StandartFieldSize);
                            break;
                        default: return;
                    }

                    void StandardInput()
                    {
                        StartValue = EditorCustomFunctions.IntInputFieldWithLabel(StartValue, $"StartValue", StandartLabelSize,StandartFieldSize);
                        ValueLowerBound = EditorCustomFunctions.IntInputFieldWithLabel(ValueLowerBound, "Value Min", StandartLabelSize,StandartFieldSize);
                        ValueUpperBound = EditorCustomFunctions.IntInputFieldWithLabel(ValueUpperBound, "Value Max", StandartLabelSize,StandartFieldSize);
                    }

                    GUILayout.Space(10);

                    if (GUILayout.Button("Apply Pattern", GUILayout.Height(25)))
                    {
                        PatternBase pattern = null;
                        switch (selectedPatternType)
                        {
                            case PatternBase.PatternTypes.DiagonalDescending:
                                pattern = new DiagonalDescendingPattern(StartValue, ValueLowerBound, ValueUpperBound);
                                break;
                            case PatternBase.PatternTypes.DiagonalRising:
                                pattern = new DiagonalRisingPattern(StartValue, ValueLowerBound, ValueUpperBound);
                                break;
                            case PatternBase.PatternTypes.Linear:
                                pattern = new LinearPattern(linearValue);
                                break;
                            case PatternBase.PatternTypes.ZigZack:
                                pattern = new ZigZackPattern(StartValue, ValueLowerBound, ValueUpperBound);
                                break;
                            case PatternBase.PatternTypes.Custom:
                                pattern = new CustomPattern(CustomPattern);
                                break;
                            case PatternBase.PatternTypes.Random:
                                pattern = new RandomPattern(MinValue, MaxValue);
                                break;
                            default: return;
                        }

                        if (pattern != null && selectedProperty != null) ApplyPattern(pattern, StepLowerBound, StepUpperBound);
                        else Debug.Log("Pattern could not be applied");
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void ApplyPattern( PatternBase pattern, int lowerBoundIndex, int upperBoundIndex)
        {
            if (lowerBoundIndex < 0) lowerBoundIndex = 0;
            int aryLength = myWavesData.Waves[IdOfSelectedWave].SpawnData.Count;
            if (upperBoundIndex > aryLength) upperBoundIndex = aryLength;
            int waveID = selectedProperty.FindPropertyRelative("ID").intValue;
            for (int i = lowerBoundIndex; i < upperBoundIndex; i++)
            {
                int patternValue = pattern.GetValue();
                myWavesData.Waves[waveID].SpawnData[i].EnemyData[patternValue] = 1;
            }
            serializedObject = new SerializedObject(serializedObject.targetObject);
        }
    }
}
