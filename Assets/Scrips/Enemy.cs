using System.Collections;
using Scrips.Background;
using Scrips.Background.Pooling;
using Unity.Mathematics;
using UnityEngine;

namespace Scrips
{
    public class Enemy : MonoBehaviour
    {
        private PathKeeper _pathKeeper;
        private StatsKeeper _statsKeeper;
        private Coroutine _currentStopEnemy, _currentDrift;
        private Vector3 _directionDriftOf;
        private int _nextPointInArry = 0;
        private float _speed;
        private bool _stop;

        private static StandardEnemyPool Pool;
        
        public int hp = 1;
        public float distance;
        
        [SerializeField] private ParticleSystem deathParticleSystem;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            if (Pool == null)
            {
                Pool = StandardEnemyPool.Instance;
            }
            _pathKeeper = PathKeeper.Instance;
            _statsKeeper = StatsKeeper.Instance;
        }

        private void Update()
        {
            if (_stop) {return; }

            if (_directionDriftOf.magnitude > 0.1f)
            {
                transform.position += _directionDriftOf * (Time.deltaTime * _speed * 0.6f); return;
            }
            transform.Translate(((_pathKeeper.PathPoints[_nextPointInArry].transform.position - transform.position).normalized) *
                                (Time.deltaTime * _speed));
            distance += Time.deltaTime * _speed;
            if (Vector3.Distance( transform.position, _pathKeeper.PathPoints[_nextPointInArry].transform.position) < 0.1f)
            {
                if (_nextPointInArry == _pathKeeper.PathPoints.Length - 1)
                {
                    _statsKeeper.Hp -= hp;
                    SpawnManager.Instance.InvokeWaveCheck();
                    
                    Pool.AddObjectToPool(gameObject);
                    return;
                }

                _nextPointInArry++;
                _directionDriftOf = Vector3.zero;
            }
        }

        public void SetColorAndSpeed()
        {
            if (hp > 0)
            {
                _spriteRenderer.color = ColorKeeper.StandardColors(hp - 1);
            }
            _speed = 1 + (hp - 1) * 0.5f;
        }

        public void TakeDamage(int damage)
        {
            hp -= damage;
            if (hp < 1)
            {
                ParticleSystem.MainModule particlesMain = Instantiate(deathParticleSystem, transform.position,
                    quaternion.identity).main;
                particlesMain.startColor = _spriteRenderer.color;
                _statsKeeper.Money += damage + hp;
                SpawnManager.Instance.InvokeWaveCheck();
                RestVariables();
                Pool.AddObjectToPool(gameObject);
                return;
            }
            else
            {
                _statsKeeper.Money += damage;
            }
            SetColorAndSpeed();
        }

        public void TriggerStopEnemy(float sec)
        {
            if (_currentStopEnemy != null)
            {
                StopCoroutine(_currentStopEnemy);
            }
            _currentStopEnemy = StartCoroutine(StopEnemy(sec));
        }

        private IEnumerator StopEnemy(float sec)
        {
            _stop = true;
            yield return new WaitForSeconds(sec);
            _stop = false;
        }

        public void ThrowBack(int pointsOnPath, Vector3 drift)
        {
            if (_currentDrift != null) return;
            _nextPointInArry -= pointsOnPath;
            if (_nextPointInArry < 1)
            { _nextPointInArry = 0; }
            _directionDriftOf = drift;
            _currentDrift = StartCoroutine(DriftTime());
        }
    
        private IEnumerator DriftTime()
        {
            yield return new WaitForSeconds(2f);
            _directionDriftOf = Vector3.zero;
            _currentDrift = null;
        }

        private void RestVariables()
        {
            hp = 1;
            distance = 0;
            _nextPointInArry = 0;
            _speed = 0;
            if (_currentStopEnemy != null)
            {
                StopCoroutine(_currentStopEnemy);
                _currentStopEnemy = null;
            }
            _stop = false;
            if (_currentDrift != null)
            {
                StopCoroutine(_currentDrift);
                _currentDrift = null;
            }
            _directionDriftOf = Vector3.zero;
        }
    }
}
