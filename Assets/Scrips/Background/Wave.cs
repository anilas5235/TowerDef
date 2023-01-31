using UnityEngine;

namespace Scrips.Background
{
   [CreateAssetMenu]
   public class Wave : ScriptableObject
   {
      public int[] SpawnAmountOfEnemys;
      public float[] SpawnDelay;
      public float[] StartSpawnIn;
   }
}
