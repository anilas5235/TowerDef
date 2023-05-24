using Background.Keeper;
using Background.WaveManaging;
using Editor.Others;
using UnityEditor;
using UnityEngine;

namespace Editor.WaveEditor
{
    [CustomPropertyDrawer(typeof(WavePoint))]
    public class WavePointCustomPropertyDrawer : PropertyDrawer
    {
        private const float FoldoutHeight = 25f;
        private SerializedProperty P_EnemyData = null;

        private static GUIStyle[] _guiStylesForButtons;
        private static int BackgroundIndicator =0;
        public static int PP_BackgroundIndicator
        {
            get => BackgroundIndicator;
            set => BackgroundIndicator = value < 1 ? 0 : 1;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return FoldoutHeight+2f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_guiStylesForButtons == null)InitializeGUIStyles();
            P_EnemyData = property.FindPropertyRelative("EnemyData");

            if (P_EnemyData.arraySize < WavePoint.WavePointSize) P_EnemyData.arraySize = WavePoint.WavePointSize; 

            EditorGUILayout.BeginHorizontal(LightDarkAlternating());
            {
                EditorGUILayout.LabelField( property.FindPropertyRelative("Name").stringValue,
                    GUILayout.MaxWidth(80),GUILayout.Height(FoldoutHeight));
                for (int i = 0; i < P_EnemyData.arraySize; i++)
                {
                    GUIStyle style = P_EnemyData.GetArrayElementAtIndex(i).intValue == 1
                        ? _guiStylesForButtons[i + 1] : _guiStylesForButtons[0];

                    if (GUILayout.Button( "", style,GUILayout.MaxWidth(FoldoutHeight),GUILayout.MaxHeight(FoldoutHeight)))
                    {
                        P_EnemyData.GetArrayElementAtIndex(i).intValue =
                            P_EnemyData.GetArrayElementAtIndex(i).intValue == 1 ? 0 : 1;
                    }
                    GUILayout.Space(4);
                }
            }
            EditorGUILayout.EndHorizontal();

            property.serializedObject.ApplyModifiedProperties();
        }

        private void InitializeGUIStyles()
        {
            _guiStylesForButtons = new GUIStyle[16];
            _guiStylesForButtons[0] = new GUIStyle
            {
                normal =
                {
                    background = EditorCustomFunctions.MakeTexture2D(50, 2, new Color(0, 0, 0, 0.1f))
                }
            };
            for (int i = 1; i < _guiStylesForButtons.Length; i++)
            {
                _guiStylesForButtons[i] = new GUIStyle
                {
                    normal =
                    {
                        background =  EditorCustomFunctions.MakeTexture2D(50, 2, ColorKeeper.StandardColors(i - 1))
                    }
                };
            }
        }

        private static GUIStyle LightDarkAlternating()
        {
            GUIStyle returnGUIStyle = BackgroundIndicator == 0
                ? EditorCustomFunctions.GetStandardGUIStyle(EditorCustomFunctions.StandardGUIStyles.LightGray)
                : EditorCustomFunctions.GetStandardGUIStyle(EditorCustomFunctions.StandardGUIStyles.DarkGray);
            BackgroundIndicator++;
            if (BackgroundIndicator > 1) BackgroundIndicator = 0;
            return returnGUIStyle;
        }
    }
}