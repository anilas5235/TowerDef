using Background.WaveManaging;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Editor
{
    public class AssetHandler
    {
        [OnOpenAsset()]
        public static bool OpenEditor(int instanceID, int line)
        {
            WavesData obj = EditorUtility.InstanceIDToObject(instanceID) as WavesData;
            if (obj != null)
            {
                WaveDataObjectEditorWindow.Open(obj);
                return true;
            }
            return false;
        }
    }
    
    [CustomEditor(typeof(WavesData))]
    public class WaveDataObjectCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor")) WaveDataObjectEditorWindow.Open((WavesData) target);
            base.OnInspectorGUI();
        }
    }
}
