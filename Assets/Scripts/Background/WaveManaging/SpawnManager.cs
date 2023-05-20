using System;
using System.Collections;
using System.Collections.Generic;
using Background.WaveManaging;
using Scrips.Background.Pooling;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Scrips.Background.WaveManaging
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance;
        public List<GameObject> activeEnemys;
        public bool waveIsRunning ;
        
        private int _currentWave = -1;
        private bool _doneSpawning = true;
        private int currentStep = 0;
        private WavePoint[] currentWaveData;

        private WavesData waves;
        [SerializeField] private Button waveStartButton;
        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            else { Destroy(this); return; }

            waves = Resources.Load<WavesData>("WaveData/Standart100");
        }

        private void Start()
        {
            if (waves.Waves.Count < 1)
            { print("no Waves defined"); }
        }

        private void Update()
        {

            if(_currentWave >= waves.Waves.Count-1 || waveIsRunning)
            {
                waveStartButton.interactable = false;
            }
            else
            {
                waveStartButton.interactable = true;
            }
        }

        private void FixedUpdate()
        {
            if (!waveIsRunning) return;
            if (!_doneSpawning) return;
            if (activeEnemys.Count < 1)
            {
                waveIsRunning = false;
            }
        }

        private IEnumerator Spawn()
        {
            _doneSpawning = false;
            if (currentStep >= currentWaveData.Length)
            {
                _doneSpawning = true;
            }
            else
            {
                int[] enemyData = currentWaveData[currentStep].EnemyData;
                for (int i = 0; i < enemyData.Length; i++)
                {
                    if (enemyData[i]==1)
                    {
                        Enemy E = StandardEnemyPool.Instance.GetObjectFromPool().GetComponent<Enemy>();
                        E.RestVariables();
                        E.gameObject.transform.position = transform.position;
                        E.hp = i;
                        E.SetColorAndSpeed();
                        E.pooled = false;
                        activeEnemys.Add(E.gameObject);
                    }
                }

                yield return new WaitForSeconds(1f + currentWaveData[currentStep].ExtraWait);
                currentStep++;
                StartCoroutine(Spawn());
            }
        }
        public void StartWave()
        {
            if (waveIsRunning)
            {
                return;
            }

            _currentWave++;
            currentWaveData = waves.Waves[_currentWave].SpawnData.ToArray();
            currentStep = 0;
            waveIsRunning = true;
            StartCoroutine(Spawn());
        }
    }
}
