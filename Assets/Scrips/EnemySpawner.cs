using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject Enemy;

    public float nextTimeToSpawn;
    public float SpawnDelay;
    public int AmountToSpawn ;
    private int SpawnedAmount;
    public SpawnManager SpawnManager;
    public StatsKeeper StatsKeeper;

    // Update is called once per frame
    void Update()
    {
        if (nextTimeToSpawn< Time.time && SpawnedAmount < AmountToSpawn )
        {
            Enemy E = Instantiate(Enemy, transform.position, quaternion.identity).GetComponent<Enemy>();
            E._statsKeeper = StatsKeeper;
            nextTimeToSpawn += SpawnDelay;
            SpawnedAmount++;
        }
        else if(AmountToSpawn <1 )
        {
            SpawnManager.IsWaveFinished();
            Destroy(this);
        }
    }

}
