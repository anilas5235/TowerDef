using System.Collections.Generic;
using Scrips.Background.WaveManaging;

namespace Background.WaveManaging
{
   [System.Serializable]
   public class Wave
   {
      public string Name;
      public int ID;
      public List<WavePoint> SpawnData;

      public void NameSteps()
      {
         for (int i = 0; i < SpawnData.Count; i++)
         {
            SpawnData[i].Name = $"Step{i}";
         }
      }
   }
}

  
