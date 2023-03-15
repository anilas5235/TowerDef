using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Scrips.Background
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;
        public bool waveIsRunning ;
        
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
            { print("no Waves defined"); }
        }

        private void Update()
        {

            if(_currentWave >= waves.Length-1 || waveIsRunning)
            {
                waveStartButton.interactable = false;
            }
            else
            {
                waveStartButton.interactable = true;
            }
            if(Input.GetButton("Jump")&& !waveIsRunning)
            { StartWave(); }
        }

        private void SetUpSpawners()
        {
            for (int i = 0; i < waves[_currentWave].SpawnAmountOfEnemys.Length; i++)
            {
                if(waves[_currentWave].SpawnAmountOfEnemys[i] <1){continue;}
                EnemySpawner spawnerRef = Instantiate(spawner, transform.position, quaternion.identity).GetComponent<EnemySpawner>();
                spawnerRef.Enemy = enemies[i];
                spawnerRef.AmountToSpawn = waves[_currentWave].SpawnAmountOfEnemys[i];
                spawnerRef.SpawnDelay =  waves[_currentWave].SpawnDelay[i];
                spawnerRef.nextTimeToSpawn = Time.time +  waves[_currentWave].StartSpawnIn[i];
                spawnerRef.SpawnManager = this;
            }
        }

        public void InvokeWaveCheck()
        {
            StartCoroutine(nameof(IsWaveFinished));
        }
        private IEnumerator IsWaveFinished()
        {
            yield return new WaitForEndOfFrame();
            if (FindObjectOfType<Enemy>() == null && FindObjectOfType<EnemySpawner>() == null) { waveIsRunning = false; }
        }

        public void StartWave()
        {
            if(waveIsRunning ){return;}
            _currentWave++; SetUpSpawners(); waveIsRunning = true;
        }
    }
}
