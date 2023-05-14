using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scrips.Background
{
    [System.Serializable]
    public class WavePoint 
    {
        public WavePoint()
        {
            EnemyData = new int[15];
            additionalWaitUntilNextWavePoint = 0;
        }
        public string Name;
        public int[] EnemyData;
        public int additionalWaitUntilNextWavePoint;
    }

    
    [CustomPropertyDrawer(typeof(WavePoint))]
    public class WavePointCustomPropertyDrawer : PropertyDrawer
    {
        private const float FoldoutHeight = 16f;
        private SerializedProperty P_EnemyData;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return FoldoutHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            P_EnemyData ??= property.FindPropertyRelative("EnemyData");
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.indentLevel++;
            EditorGUI.LabelField( new Rect(position.x,position.y,100,FoldoutHeight),property.FindPropertyRelative("Name").stringValue);
            float addX = 101;
            float offsetX = 4f;

            for (int i = 0; i < P_EnemyData.arraySize; i++)
            {
                Rect rect = new Rect(position.x + addX, position.y, FoldoutHeight, FoldoutHeight);
                Color color = P_EnemyData.GetArrayElementAtIndex(i).intValue == 1 ? ColorKeeper.StandardColors(i) : new Color(0, 0, 0, 0.1f);
                GUIStyle style = new GUIStyle();
                style.normal.background = MakeTexture2D(50, 2, color);
                if (GUI.Button(rect, "", style))
                {
                    P_EnemyData.GetArrayElementAtIndex(i).intValue = P_EnemyData.GetArrayElementAtIndex(i).intValue == 1 ? 0 : 1;
                }

                addX += FoldoutHeight + offsetX;
            }

            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }

        private Texture2D MakeTexture2D(int width, int height, Color col)
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
