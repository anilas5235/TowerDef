using System;
using System.Collections;
using System.Collections.Generic;
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

        [SerializeField] private Wave[] waves;
        [SerializeField] private Button waveStartButton;
        [SerializeField] private LayerMask enemy;
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
                foreach (var hpValue in currentWaveData[currentStep].hp)
                {
                    if (hpValue > 0 && hpValue < 10)
                    {
                        Enemy E = StandardEnemyPool.Instance.GetObjectFromPool().GetComponent<Enemy>();
                        E.RestVariables();
                        E.gameObject.transform.position = transform.position;
                        E.hp = hpValue;
                        E.SetColorAndSpeed();
                        E.pooled = false;
                        activeEnemys.Add(E.gameObject);
                    }
                }

                yield return new WaitForSeconds(1f + currentWaveData[currentStep].additionalWaitUntilNextWavePoint);
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
            currentWaveData = waves[_currentWave].SpawnData;
            currentStep = 0;
            waveIsRunning = true;
            StartCoroutine(Spawn());
        }
    }
}
