using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(this); }
    }

    [SerializeField] private GameObject[] Enemys;
    [SerializeField] private GameObject Spawner;
    private int currentWave = -1;
    [SerializeField] private Wave[] Waves;
    public bool WaveIsRunning = false;
    
    public bool wavesFinished = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if (Waves.Length < 1)
        { print("no Waves defined"); wavesFinished = true; }
        else if (currentWave >= Waves.Length)
        { print("waves finished"); wavesFinished = true; }
    }

    // Update is called once per frame
    void Update()
    {
        if(wavesFinished ){return;}
        
        if(currentWave >= Waves.Length-1)
        {
            print("waves finished");
            wavesFinished = true;
        }
        if(Input.GetButton("Jump")&& !WaveIsRunning)
        { StartWave(); }
    }

    private void SetUpSpawners()
    {
        for (int i = 0; i < Waves[currentWave].SpawnAmountOfEnemys.Length; i++)
        {
            EnemySpawner SpawnerRef = Instantiate(Spawner, transform.position, quaternion.identity).GetComponent<EnemySpawner>();
            SpawnerRef.Enemy = Enemys[i];
            SpawnerRef.AmountToSpawn = Waves[currentWave].SpawnAmountOfEnemys[i];
            SpawnerRef.SpawnDelay =  Waves[currentWave].SpawnDelay[i];
            SpawnerRef.nextTimeToSpawn = Time.time +  Waves[currentWave].StartSpawnIn[i];
            SpawnerRef.SpawnManager = this;
            if(Waves[currentWave].SpawnAmountOfEnemys[i] <1){Destroy(SpawnerRef.gameObject); return;}
        }
    }

    public void InvokeWaveCheck()
    {
        Invoke("IsWaveFinished",0.2f);
    }
    private void IsWaveFinished()
    {
        if (FindObjectOfType<EnemySpawner>() == null) { print("Wave is finished"); WaveIsRunning = false; }
    }

    public void StartWave()
    {
        if(wavesFinished || WaveIsRunning ){return;}
        currentWave++; SetUpSpawners(); WaveIsRunning = true;
    }
}
