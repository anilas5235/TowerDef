using System.Collections;
using System.Collections.Generic;
using Background.Keeper;
using Background.Pooling;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Background.WaveManaging
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;
        public List<GameObject> activeEnemies;
        public bool waveIsRunning ;

        private int _currentWave = -1;
        private bool _doneSpawning = true;
        private int currentStep = 0;
        private WavePoint[] currentWaveData;

        [SerializeField] private WavesData waves;
        [SerializeField] private Button waveStartButton;
        [SerializeField] private bool infiniteSpawn = false;
        private void Awake()
        {
            if (Instance == null) Instance = this; 
            else  Destroy(this);
        }

        private void Start()
        {
            if (waves.Waves.Count < 1)
            { print("no Waves defined"); }

            if (infiniteSpawn) StartCoroutine(StartWaveAfter(4));
        }

        private void Update()
        {
            if (waveStartButton) waveStartButton.interactable = _currentWave < waves.Waves.Count - 1 && !waveIsRunning;
        }

        private void FixedUpdate()
        {
            if (!waveIsRunning ||!_doneSpawning) return;
            if (activeEnemies.Count < 1)
            {
                waveIsRunning = false;
                if (infiniteSpawn) StartWave();
            }
           
        }

        private IEnumerator Spawn()
        {
            _doneSpawning = false;
            if (currentStep >= currentWaveData.Length) _doneSpawning = true;
            else
            {
                int[] enemyData = currentWaveData[currentStep].EnemyData;
                for (int i = 0; i < enemyData.Length; i++)
                {
                    if (enemyData[i]==1)
                    {
                        Enemy E = StandardEnemyPool.Instance.GetObjectFromPool().GetComponent<Enemy>();
                        E.RestVariables();
                        E.gameObject.transform.position = PathKeeper.Instance.PathPoints[0];
                        E.hp = i+1;
                        E.SetColorAndSpeed();
                        E.pooled = false;
                        activeEnemies.Add(E.gameObject);
                    }
                }

                yield return new WaitForSeconds(1f + currentWaveData[currentStep].ExtraWait);
                currentStep++;
                StartCoroutine(Spawn());
            }
        }

        public void StartWave()
        {
            if (waveIsRunning) return;


            _currentWave++;
            if (infiniteSpawn && _currentWave >= waves.Waves.Count)
            {
                RestartWave();
                return;
            }

            currentWaveData = waves.Waves[_currentWave].SpawnData.ToArray();
            currentStep = 0;
            waveIsRunning = true;
            StartCoroutine(Spawn());
        }

        private void RestartWave()
        {
            _currentWave = 0;
            currentWaveData = waves.Waves[_currentWave].SpawnData.ToArray();
            currentStep = 0;
            waveIsRunning = true;
            StartCoroutine(Spawn());
        }

        private IEnumerator StartWaveAfter(float time)
        {
            yield return new WaitForSeconds(time);
            StartWave();
        }
    }
}
