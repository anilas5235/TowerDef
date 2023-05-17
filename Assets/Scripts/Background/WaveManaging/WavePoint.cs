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
        private const float FoldoutHeight = 20f;
        private SerializedProperty P_EnemyData = null;

        private static GUIStyle[] _guiStylesForButtons;
        private static int BackgroundIndicator =0;
        public static int PP_BackgroundIndicator
        {
            get => BackgroundIndicator;
            set => BackgroundIndicator = value < 1 ? 0 : 1;
        }

        private static Color light = new Color(1, 1, 1, 0.2f), dark = new Color(1, 1, 1, 0f);
            

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return FoldoutHeight+2f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_guiStylesForButtons == null)InitializeGUIStyles();
            P_EnemyData = property.FindPropertyRelative("EnemyData");

            if (P_EnemyData.arraySize < 15) { P_EnemyData.arraySize = 15; }
            
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.indentLevel++;
            
            EditorGUI.DrawRect(position, LightDarkAlternating());
            EditorGUI.LabelField( new Rect(position.x,position.y,100,FoldoutHeight),property.FindPropertyRelative("Name").stringValue);
            float addX = 101;
            float offsetX = 4f;

            for (int i = 0; i < P_EnemyData.arraySize; i++)
            {
                Rect rect = new Rect(position.x + addX, position.y+2f, FoldoutHeight, FoldoutHeight);
                GUIStyle style = P_EnemyData.GetArrayElementAtIndex(i).intValue == 1 ? _guiStylesForButtons[i+1] : _guiStylesForButtons[0];
                
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

        private void InitializeGUIStyles()
        {
            _guiStylesForButtons = new GUIStyle[16];
            _guiStylesForButtons[0] = new GUIStyle
            {
                normal =
                {
                    background = MakeTexture2D(50, 2, new Color(0, 0, 0, 0.1f))
                }
            };
            for (int i = 1; i < _guiStylesForButtons.Length; i++)
            {
                _guiStylesForButtons[i] = new GUIStyle
                {
                    normal =
                    {
                        background = MakeTexture2D(50, 2, ColorKeeper.StandardColors(i - 1))
                    }
                };
            }
        }

        private Color LightDarkAlternating()
        {
            Color color = BackgroundIndicator == 0 ? light : dark;
            BackgroundIndicator++;
            if (BackgroundIndicator>1) BackgroundIndicator = 0;

            return color;
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
