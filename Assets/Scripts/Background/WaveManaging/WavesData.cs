using System.Collections.Generic;
using UnityEngine;

namespace Background.WaveManaging
{
    [CreateAssetMenu]
    public class WavesData : ScriptableObject
    {
        public List<Wave> Waves = new List<Wave>();

        public void NameWaves()
        {
            for (int i = 0; i < Waves.Count; i++)
            {
                Waves[i].Name = $"Wave{i}";
                Waves[i].ID = i;
            }
        }

        public void AddWave()
        {
            Waves.Add(new Wave());
        }

        public void RemoveAt(int index)
        {
            Waves.RemoveAt(index);
        }
    }
}