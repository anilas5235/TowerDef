using Unity.Mathematics;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Scrips.Background
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;
        public bool waveIsRunning = false;
        public bool wavesFinished = false;
        
        private int _currentWave = -1;
        
        [SerializeField] private GameObject[] enemies;
        [SerializeField] private GameObject spawner;
        [SerializeField] private Wave[] waves;
        [SerializeField] private Button waveStartButton;
        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            else { Destroy(this); }
        }

        private void Start()
        {
            if (waves.Length < 1)
            { print("no Waves defined"); wavesFinished = true; }
        }

        private void Update()
        {
            if(wavesFinished ){return;}
        
            if(_currentWave >= waves.Length-1)
            {
                print("waves finished");
                wavesFinished = true;
                waveStartButton.interactable = false;
            }
            if(Input.GetButton("Jump")&& !waveIsRunning)
            { StartWave(); }
        }

        private void SetUpSpawners()
        {
            for (int i = 0; i < waves[_currentWave].SpawnAmountOfEnemys.Length; i++)
            {
                EnemySpawner spawnerRef = Instantiate(spawner, transform.position, quaternion.identity).GetComponent<EnemySpawner>();
                spawnerRef.Enemy = enemies[i];
                spawnerRef.AmountToSpawn = waves[_currentWave].SpawnAmountOfEnemys[i];
                spawnerRef.SpawnDelay =  waves[_currentWave].SpawnDelay[i];
                spawnerRef.nextTimeToSpawn = Time.time +  waves[_currentWave].StartSpawnIn[i];
                spawnerRef.SpawnManager = this;
                if(waves[_currentWave].SpawnAmountOfEnemys[i] <1){Destroy(spawnerRef.gameObject); return;}
            }
        }

        public void InvokeWaveCheck()
        {
            Invoke(nameof(IsWaveFinished),0.2f);
        }
        private void IsWaveFinished()
        {
            if (FindObjectOfType<EnemySpawner>() == null) { print("Wave is finished"); waveIsRunning = false; }
        }

        public void StartWave()
        {
            if(wavesFinished || waveIsRunning ){return;}
            _currentWave++; SetUpSpawners(); waveIsRunning = true;
        }
    }
}
