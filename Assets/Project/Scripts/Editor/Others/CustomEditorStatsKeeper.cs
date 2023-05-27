using Background.Keeper;
using UnityEditor;
using UnityEngine;

namespace Editor.Others
{
    [CustomEditor(typeof(StatsKeeper))]
    public class CustomEditorStatsKeeper : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            StatsKeeper script = (StatsKeeper)target;
            base.OnInspectorGUI();
            if (GUILayout.Button("Add 10000 Money"))
            {
                script.Money += 10000;
            }
            if (GUILayout.Button("-100 hp"))
            {
                script.Hp -= 100;
            }
        }
    }
}
