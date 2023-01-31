using Unity.Mathematics;
using UnityEngine;

namespace Scrips.Background
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameObject Enemy;

        public float nextTimeToSpawn;
        public float SpawnDelay;
        public int AmountToSpawn ;
        private int SpawnedAmount;
        public SpawnManager SpawnManager = SpawnManager.Instance;

        private void OnEnable()
        {
            SpawnManager = SpawnManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            if (nextTimeToSpawn< Time.time && SpawnedAmount < AmountToSpawn )
            {
                Enemy E = Instantiate(Enemy, transform.position, quaternion.identity).GetComponent<Enemy>();
                nextTimeToSpawn += SpawnDelay;
                SpawnedAmount++;
            }
            else if(SpawnedAmount >= AmountToSpawn )
            {
                SpawnManager.InvokeWaveCheck();
                Destroy(this.gameObject);
            }
        }

    }
}
