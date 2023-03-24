using System.Collections;
using Scrips.Background.Pooling;
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
        private bool _doneSpawning = true;
        private int currentStep = 0;
        private WavePoint[] currentWaveData;
        
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
                foreach (var i in currentWaveData[currentStep].hp)
                {
                    if (i > 0 && i < 10)
                    {
                        Enemy E = StandardEnemyPool.Instance.GetObjectFromPool().GetComponent<Enemy>();
                        E.RestVariables();
                        E.gameObject.transform.position = transform.position;
                        E.hp = i;
                        E.SetColorAndSpeed();
                    }
                }

                yield return new WaitForSeconds(1f + currentWaveData[currentStep].additionalWaitUntilNextWavePoint);
                currentStep++;
                StartCoroutine(Spawn());
            }
        }

        public void InvokeWaveCheck()
        {
            StartCoroutine(nameof(IsWaveFinished));
        }
        private IEnumerator IsWaveFinished()
        {
            yield return new WaitForEndOfFrame();
            if (FindObjectOfType<Enemy>() == null && _doneSpawning) { waveIsRunning = false; }
        }

        public void StartWave()
        {
            if (waveIsRunning)
            {
                return;
            }

            _currentWave++;
            currentWaveData = waves[_currentWave].SpawnData;
            StartCoroutine(Spawn());
            waveIsRunning = true;
        }
    }
}
