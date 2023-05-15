using Background.WaveManaging;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class WaveDataObjectEditorWindow : ExtendedEditorWindow
    {
        private WavesData myWavesData;
        
        private Vector2 WaveScrollPosition = Vector2.zero, ToolBarScrollPosition = Vector2.zero;

        private GUIStyle Test = new GUIStyle();

        public static void Open(WavesData dataObject)
        {
            WaveDataObjectEditorWindow window = GetWindow<WaveDataObjectEditorWindow>("Waves Data Editor");
            window.serializedObject = new SerializedObject(dataObject);
            window.minSize = new Vector2(850, 200);
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
            selectedProperty = null;
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
            Debug.Log("NameStepsCalled");

            serializedObject.ApplyModifiedProperties();
            SaveChanges();
            serializedObject = new SerializedObject(serializedObject.targetObject);

            SerializedProperty thisSpawnData = currentProperty.FindPropertyRelative("SpawnData");

            thisSpawnData.isExpanded = true;

            EditorGUILayout.PropertyField(thisSpawnData,true);
           
            EditorGUILayout.EndScrollView();
        }

        private void DrawToolBar()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.MinWidth(150), GUILayout.ExpandHeight(true));
            EditorGUILayout.LabelField("Tool Box",EditorStyles.boldLabel,GUILayout.MaxWidth(150));
            ToolBarScrollPosition = EditorGUILayout.BeginScrollView(ToolBarScrollPosition, "box");
            if (GUILayout.Button("Add Wave",GUILayout.Height(40f))) AddWaveObject();
            if (selectedProperty != null)
            {
                if (GUILayout.Button("Delete Wave",GUILayout.Height(40f))) RemoveWaveObject(selectedProperty.FindPropertyRelative("ID").intValue);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawWaveSectionBar()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.MinWidth(150), GUILayout.ExpandHeight(true));
            EditorGUILayout.LabelField("Wave Section",EditorStyles.boldLabel,GUILayout.MaxWidth(150));
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
    }
}
