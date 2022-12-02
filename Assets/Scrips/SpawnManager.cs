using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField] private GameObject[] Enemys;
    [SerializeField] private GameObject Spawner;
    private int currentWave = 0;
    [SerializeField] private Wave[] Waves;
    public bool WaveIsRunning = false;
    private StatsKeeper StatsKeeper;
    private int SpawnersActive = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(Waves.Length <1){print("no Waves defined");}
        print("Enemy arry leangth "+ Enemys.Length );
        StatsKeeper = FindObjectOfType<StatsKeeper>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Jump")&& !WaveIsRunning){SetUpSpawners(); WaveIsRunning = true; }
    }

    private void SetUpSpawners()
    {
        SpawnersActive = Enemys.Length;
        for (int i = 0; i < Waves[currentWave].SpawnAmountOfEnemys.Length; i++)
        {
            EnemySpawner SpawnerRef = Instantiate(Spawner, transform.position, quaternion.identity).GetComponent<EnemySpawner>();
            SpawnersActive++;
            SpawnerRef.Enemy = Enemys[i];
            SpawnerRef.AmountToSpawn = Waves[currentWave].SpawnAmountOfEnemys[i];
            SpawnerRef.SpawnDelay =  Waves[currentWave].SpawnDelay[i];
            SpawnerRef.nextTimeToSpawn = Time.time +  Waves[currentWave].StartSpawnIn[i];
            SpawnerRef.StatsKeeper = StatsKeeper;
            if(Waves[currentWave].SpawnAmountOfEnemys[i] <1){Destroy(SpawnerRef.gameObject); SpawnersActive--; return;}
        }
    }

    public void IsWaveFinished()
    {
        SpawnersActive--;
        if (SpawnersActive < 1) { print("Wave is finished"); WaveIsRunning = false; }
    }
}
