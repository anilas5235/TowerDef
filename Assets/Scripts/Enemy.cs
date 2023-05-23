using System.Collections;
using Background.Keeper;
using Background.Pooling;
using Background.WaveManaging;
using Scrips.Background;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private PathKeeper _pathKeeper;
    private StatsKeeper _statsKeeper;
    private Coroutine _currentStopEnemy;
    private Vector3 _positionBeforeDriftOff;
    private int _nextPointInArry = 0;
    private float _speed;
    private EnemyBehaviourStates myBehaviourState;

    private static StandardEnemyPool Pool;
        
    public int hp = 1;
    public float distance;
    public bool pooled;
        
    [SerializeField] private ParticleSystem deathParticleSystem;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private enum EnemyBehaviourStates
    {
        Normal = 0,
        Stop = 1,
        Drift = 2,
        RecoveringFormDrift =3,
    }
    private void Start()
    {
        if (Pool == null) Pool = StandardEnemyPool.Instance;
        _pathKeeper = PathKeeper.Instance;
        _statsKeeper = StatsKeeper.Instance;
        myBehaviourState = EnemyBehaviourStates.Normal;
    }

    private void Update()
    {
        if (pooled) {return; }

        switch (myBehaviourState)
        {
            case EnemyBehaviourStates.Normal:
                transform.Translate(((_pathKeeper.PathPoints[_nextPointInArry] - transform.position).normalized) *
                                    (Time.deltaTime * _speed));
                if (Vector3.Distance(transform.position, _pathKeeper.PathPoints[_nextPointInArry]) < 0.1f)
                {
                    if (_nextPointInArry == _pathKeeper.PathPoints.Length - 1)
                    {
                        _statsKeeper.Hp -= hp;
                        Death();
                        return;
                    }

                    _nextPointInArry++;
                }

                distance += Time.deltaTime * _speed;

                break;
            case EnemyBehaviourStates.Stop: return;
            case EnemyBehaviourStates.Drift: return;
            case EnemyBehaviourStates.RecoveringFormDrift:
                transform.Translate((_positionBeforeDriftOff - transform.position).normalized *
                                    (Time.deltaTime * _speed));
                if (Vector3.Distance(_positionBeforeDriftOff, transform.position) < 0.1f)
                {
                    myBehaviourState = EnemyBehaviourStates.Normal;
                }

                distance += Time.deltaTime * _speed;
                break;
        }
    }

    public void SetColorAndSpeed()
    {
        if (hp > 0) _spriteRenderer.color = ColorKeeper.StandardColors(hp - 1);
        _speed = 1 + (hp - 1) * 0.4f;
    }

    public void TakeDamage(int damage)
    {
        if (damage < 1 || hp < 1) { return; }
        hp -= damage;
        if (hp < 1)
        {
            ParticleSystem.MainModule particlesMain = Instantiate(deathParticleSystem, transform.position,
                quaternion.identity).main;
            particlesMain.startColor = _spriteRenderer.color;
            _statsKeeper.Money += damage + hp;
            Death();
            return;
        }
        else _statsKeeper.Money += damage;
        SetColorAndSpeed();
    }

    public void TriggerStopEnemy(float sec)
    {
        if (_currentStopEnemy != null)StopCoroutine(_currentStopEnemy);
        _currentStopEnemy = StartCoroutine(StopEnemy(sec));
    }

    private IEnumerator StopEnemy(float sec)
    {
        Freeze();
        yield return new WaitForSeconds(sec);
        UnFreeze();
    }

    private void Freeze() => myBehaviourState = EnemyBehaviourStates.Stop;

    private void UnFreeze() => myBehaviourState = EnemyBehaviourStates.Normal;

    public void StartDrift()
    {
        _positionBeforeDriftOff = transform.position;
        myBehaviourState = EnemyBehaviourStates.Drift;
    }

    public void StopDrift() => myBehaviourState = EnemyBehaviourStates.RecoveringFormDrift;

    public void RestVariables()
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

        myBehaviourState = EnemyBehaviourStates.Normal;
    }

    private void Death()
    {
        pooled = true;
        SpawnManager.Instance.activeEnemies.Remove(this.gameObject);
        Pool.AddObjectToPool(gameObject);
    }
}