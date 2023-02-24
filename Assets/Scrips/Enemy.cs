using System.Collections;
using Scrips.Background;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private PathKeeper _pathKeeper;
    private StatsKeeper _statsKeeper;
    private SpriteRenderer _spriteRenderer;
    private Coroutine _currentStopEnemy;
    private int _nextPointInArry = 0;
    private float _speed;
    private bool _stop;
    private Vector3 _dirctionDriftOf;
    
    public int hp = 1;
    public float distance =0;
    
    [SerializeField] private ParticleSystem deathParticleSystem;

    // Start is called before the first frame update
    private void Start()
    {
        _pathKeeper = PathKeeper.Instance;
        _statsKeeper = StatsKeeper.Instance;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        SetColorAndSpeed();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_stop) {return; }
        transform.Translate(((_pathKeeper.PathPoints[_nextPointInArry].transform.position - transform.position).normalized +_dirctionDriftOf) * (Time.deltaTime * _speed));
        distance += Time.deltaTime * _speed;
        if (Vector3.Distance( transform.position, _pathKeeper.PathPoints[_nextPointInArry].transform.position) < 0.1f)
        {
            if (_nextPointInArry == _pathKeeper.PathPoints.Length -1)
            { _statsKeeper.hp -= hp; _statsKeeper.UpdateUI(); Destroy(this.gameObject); return;}
            _nextPointInArry++;
            _dirctionDriftOf = Vector3.zero;
        }
    }

    private void SetColorAndSpeed()
    {
        switch (hp)
        {
            case 1: _spriteRenderer.color = Color.red; _speed = 1; break;
            case 2: _spriteRenderer.color = Color.blue; _speed = 1.5f; break;
            case 3: _spriteRenderer.color = Color.green; _speed = 2f; break;
            case 4: _spriteRenderer.color = Color.yellow; _speed = 2.5f; break;
            case 5: _spriteRenderer.color = Color.cyan; _speed = 3f; break;
            case 6: _spriteRenderer.color = Color.grey; _speed = 3.5f; break;
            case 7: _spriteRenderer.color = Color.black; _speed = 4f; break;
            case 8: _spriteRenderer.color = Color.white; _speed = 4.5f; break;
            
            default: print("Color for "+hp+ " hp is not defined"); break;
        }
    }

    public void TakeDamage(int Damage)
    {
        hp -= Damage;
        if (hp < 1)
        {
            ParticleSystem.MainModule particlesMain = Instantiate(deathParticleSystem, transform.position, quaternion.identity).main;
            particlesMain.startColor = _spriteRenderer.color;
            _statsKeeper.Money += Damage + hp;
            _statsKeeper.UpdateUI();
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _statsKeeper.Money += Damage;
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
        _nextPointInArry -= pointsOnPath;
        _dirctionDriftOf = drift;
        StartCoroutine(DriftTime());
    }
    
    private IEnumerator DriftTime()
    {
        yield return new WaitForSeconds(2f);
        _dirctionDriftOf = Vector3.zero;
    }
}
