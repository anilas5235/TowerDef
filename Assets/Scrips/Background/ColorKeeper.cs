using UnityEngine;

namespace Scrips.Background
{
    public class ColorKeeper : MonoBehaviour
    {
        private static Color[] StandardColorsArry =
            { Color.red, Color.blue, Color.green, Color.yellow, Color.cyan, Color.gray, Color.black, Color.white };
            
        public static Color StandardColors(int index)
        {
            if (index < 0) index = 0;
            if (index > StandardColorsArry.Length - 1) index = StandardColorsArry.Length - 1;
            return StandardColorsArry[index];
        }
    }
}
