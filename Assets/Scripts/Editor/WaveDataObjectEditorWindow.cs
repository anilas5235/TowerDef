using System;
using System.Collections.Generic;
using System.Linq;
using Background.WaveManaging;
using Others;
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
        private int MinValue, MaxValue, StartValue, StepLowerBound, StepUpperBound, linearValue,ValueLowerBound,ValueUpperBound;//Pattern Input vars
        private int InputValueWaveSize, SizeOfCustomPattern = 5;
        private int[] CustomPattern;
        private int IdOfSelectedWave = -1;
        private int StandartLabelSize = 70;
        private WavePoint[] CopiedWaveData;
        private CustomPatternSave loadedCustomPatternSave;

        public static void Open(WavesData dataObject)
        {
            WaveDataObjectEditorWindow window = GetWindow<WaveDataObjectEditorWindow>("Waves Data Editor");
            window.serializedObject = new SerializedObject(dataObject);
            window.minSize = new Vector2(800, 200);
        }

        private void OnEnable()
        {
            CustomPattern = new int[SizeOfCustomPattern];
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
                EditorGUILayout.BeginHorizontal("box");
                {
                    EditorGUILayout.LabelField(currentProperty.displayName, EditorStyles.boldLabel,
                        GUILayout.MaxWidth(70));
                    
                    InputValueWaveSize = InputFieldWithLabel(InputValueWaveSize, "Size", 50);
                    if (GUILayout.Button("Apply", GUILayout.MaxWidth(50)))
                        OverrideWaveDataLength(IdOfSelectedWave,InputValueWaveSize);
                }
                EditorGUILayout.EndHorizontal();

                SerializedProperty thisSpawnData = currentProperty.FindPropertyRelative("SpawnData");

                EditorGUILayout.BeginVertical();
                {
                    DrawProperties(thisSpawnData, true);
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
                    if (GUILayout.Button("Delete Wave", GUILayout.Height(30f), GUILayout.MaxWidth(sizeOfToolBarButtons)))
                        RemoveWaveObject(IdOfSelectedWave);
                    if (GUILayout.Button("Clear Wave Data", GUILayout.Height(30f), GUILayout.MaxWidth(sizeOfToolBarButtons)))
                        ClearDataOfWave(IdOfSelectedWave);
                    if (GUILayout.Button("Copy Wave Data", GUILayout.Height(30f), GUILayout.MaxWidth(sizeOfToolBarButtons)))
                        CopyWaveDataOfWave(IdOfSelectedWave);
                    if (CopiedWaveData != null)
                    {
                        if (GUILayout.Button("Past Wave Data", GUILayout.Height(30f), GUILayout.MaxWidth(sizeOfToolBarButtons)))
                            PasteWaveDataToWave(IdOfSelectedWave);
                    }

                    if (myWavesData.Waves[IdOfSelectedWave].SpawnData.Count > 0)
                    {
                        if (GUILayout.Button("Delete Wave Data", GUILayout.Height(30f), GUILayout.MaxWidth(sizeOfToolBarButtons)))
                            OverrideWaveDataLength(IdOfSelectedWave,0);
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
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true),GUILayout.MinWidth(150+(18+4)*15)); // ToDo: make calculation based an vars not hardcoded
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

                    StepLowerBound = InputFieldWithLabel(StepLowerBound, "From Step", StandartLabelSize);
                    StepUpperBound = InputFieldWithLabel(StepUpperBound, "To Step", StandartLabelSize);

                    switch (selectedPatternType)
                    {
                        case PatternBase.PatternTypes.DiagonalDescending:
                            StandardInput();
                            break;
                        case PatternBase.PatternTypes.DiagonalRising:
                            StandardInput();
                            break;
                        case PatternBase.PatternTypes.Linear:
                            linearValue = InputFieldWithLabel(linearValue, "Value", StandartLabelSize);
                            break;
                        case PatternBase.PatternTypes.ZigZack:
                            StandardInput();
                            break;
                        case PatternBase.PatternTypes.Custom:
                          
                            SizeOfCustomPattern = InputFieldWithLabel(SizeOfCustomPattern, "Size", StandartLabelSize);

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
                            MinValue = InputFieldWithLabel(MinValue, $"MinValue", StandartLabelSize);
                            MaxValue = InputFieldWithLabel(MaxValue, $"MaxValue", StandartLabelSize);
                            break;
                        default: return;
                    }

                    void StandardInput()
                    {
                        StartValue = InputFieldWithLabel(StartValue, $"StartValue", StandartLabelSize);
                        ValueLowerBound = InputFieldWithLabel(ValueLowerBound, "Value Min", StandartLabelSize);
                        ValueUpperBound = InputFieldWithLabel(ValueUpperBound, "Value Max", StandartLabelSize);
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
                myWavesData.Waves[waveID].SpawnData[i].EnemyData[pattern.GetValue()] = 1;
            }
            serializedObject = new SerializedObject(serializedObject.targetObject);
        }

        private static int InputFieldWithLabel(int value,string labelName,int labelSize)
        {
            EditorGUILayout.BeginHorizontal(EditorCustomFunctions.GetStandardGUIStyle(EditorCustomFunctions.StandardGUIStyles.LightGray));
            {
                EditorGUILayout.LabelField(labelName + ":", GUILayout.MaxWidth(labelSize));
                if (Int32.TryParse(EditorGUILayout.TextArea($"{value}", GUILayout.MaxWidth(70)), out var newVal))
                {
                    value = newVal;
                }
            }
            EditorGUILayout.EndHorizontal();
            return value;
        }
    }
}
