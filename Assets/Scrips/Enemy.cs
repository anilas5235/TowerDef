using Scrips.Background;
using Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private PathKeeper PathKeeper;
    private StatsKeeper StatsKeeper;
    private int nextPointInArry = 0;
    public int hp = 1;
    private SpriteRenderer SpriteRenderer;
    private float speed;
    public float distance =0;
    [SerializeField] private ParticleSystem deathParticleSystem;

    // Start is called before the first frame update
    void Start()
    {
        PathKeeper = PathKeeper.Instance;
        StatsKeeper = StatsKeeper.Instance;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        SetColorAndSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate((PathKeeper.PathPoints[nextPointInArry].transform.position - transform.position).normalized * (Time.deltaTime * speed));
        distance += Time.deltaTime * speed;
        if (Vector3.Distance( transform.position, PathKeeper.PathPoints[nextPointInArry].transform.position) < 0.1f)
        {
            if (nextPointInArry == PathKeeper.PathPoints.Length -1)
            { StatsKeeper.hp -= hp; StatsKeeper.UpdateUI(); Destroy(this.gameObject); return;}
            nextPointInArry++;
        }
    }

    private void SetColorAndSpeed()
    {
        switch (hp)
        {
            case 1: SpriteRenderer.color = Color.red; speed = 1; break;
            case 2: SpriteRenderer.color = Color.blue; speed = 1.5f; break;
            case 3: SpriteRenderer.color = Color.green; speed = 2f; break;
            case 4: SpriteRenderer.color = Color.yellow; speed = 2.5f; break;
            case 5: SpriteRenderer.color = Color.cyan; speed = 3f; break;
            case 6: SpriteRenderer.color = Color.grey; speed = 3.5f; break;
            case 7: SpriteRenderer.color = Color.black; speed = 4f; break;
            case 8: SpriteRenderer.color = Color.white; speed = 4.5f; break;
            
            default: print("Color for "+hp+ " hp is not defined"); break;
        }
    }

    public void TakeDamage(int Damage)
    {
        hp -= Damage;
        if (hp < 1)
        {
            ParticleSystem.MainModule particlesMain = Instantiate(deathParticleSystem, transform.position, quaternion.identity).main;
            particlesMain.startColor = SpriteRenderer.color;
            StatsKeeper.Money += Damage + hp;
            StatsKeeper.UpdateUI();
            Destroy(this.gameObject);
            return;
        }
        else
        {
            StatsKeeper.Money += Damage;
        }
        SetColorAndSpeed();
    }
}
