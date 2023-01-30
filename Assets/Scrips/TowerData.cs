using UnityEngine;
using UnityEngine.Serialization;

namespace Scrips
{
    [CreateAssetMenu]
    public class TowerData : ScriptableObject
    {
        public int placingCosts;
        public int[] upgradeCostsSlot0;
        public int[] upgradeCostsSlot1;
        public int[] upgradeCostsSlot2;
        public string[] statNames;
        public string towerName;
    }
}
