using System;
using UnityEngine;

namespace Others
{
    public static class EditorCustomFunctions
    {
        private static Color Color_lightGray = new Color(1, 1, 1, 0.2f), Color_darkGray = new Color(1, 1, 1, 0.05f);
        private static GUIStyle GUIStyle_lightGray, GuiStyle_darkGray;
        
        public enum StandardGUIStyles
        {
            LightGray =0,
            DarkGray =1,
        }
        public enum StandardColors
        {
            LightGray =0,
            DarkGray =1,
        }
        public static Texture2D MakeTexture2D(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        public static GUIStyle GetStandardGUIStyle(StandardGUIStyles style)
        {
            GUIStyle returnGUIStyle = new GUIStyle();
            if (GUIStyle_lightGray == null)InitializeGUIStyles();
                switch (style)
            {
                case StandardGUIStyles.LightGray:
                    returnGUIStyle = GUIStyle_lightGray;
                    break;
                case StandardGUIStyles.DarkGray:
                    returnGUIStyle = GuiStyle_darkGray;
                    break;
            }

            return returnGUIStyle;
        }

        public static Color GetStandardColor(StandardColors color)
        {
            Color returnColor = Color.white;

            switch (color)
            {
                case StandardColors.LightGray:
                    returnColor = Color_lightGray;
                    break;
                case StandardColors.DarkGray:
                    returnColor = Color_darkGray;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }

            return returnColor;
        }

        private static void InitializeGUIStyles()
        {
            GUIStyle_lightGray = new GUIStyle
            {
                normal =
                {
                    background = MakeTexture2D(1, 1, Color_lightGray)
                }
            };

            GuiStyle_darkGray = new GUIStyle
            {
                normal =
                {
                    background = MakeTexture2D(1, 1, Color_darkGray)
                }
            };
        }
    }
}