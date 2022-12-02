using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
   
    private PathKeeper PathKeeper;
    private int nextPointInArry = 0;
    public int hp = 1;
    private SpriteRenderer SpriteRenderer;
    public StatsKeeper _statsKeeper;

    private int d = 99999;

    // Start is called before the first frame update
    void Start()
    {
        PathKeeper = GameObject.FindObjectOfType<PathKeeper>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        SetColor();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate((PathKeeper.PathPoints[nextPointInArry].transform.position - transform.position).normalized * Time.deltaTime * hp);
        if (Vector3.Distance( transform.position, PathKeeper.PathPoints[nextPointInArry].transform.position) < 0.1f)
        {
            if (nextPointInArry == PathKeeper.PathPoints.Length -1)
            { _statsKeeper.hp -= hp; TakeDamage(d); return;}
            nextPointInArry++;
            //print(""+PathKeeper.PathPoints[nextPointInArry].transform.position);
        }
       
    }

    private void SetColor()
    {
        switch (hp)
        {
            case 1: SpriteRenderer.color = Color.red; break;
            case 2: SpriteRenderer.color = Color.blue; break;
            case 3: SpriteRenderer.color = Color.green; break;
            case 4: SpriteRenderer.color = Color.yellow; break;
            case 5: SpriteRenderer.color = Color.cyan; break;
            case 6: SpriteRenderer.color = Color.grey; break;
            case 7: SpriteRenderer.color = Color.black; break;
            case 8: SpriteRenderer.color = Color.white; break;
            
            default: print("Color for "+hp+ " hp is not defined"); break;
        }
    }

    public void TakeDamage(int Damage)
    {
        hp -= Damage;
        if (hp <1)
        { Destroy(this.gameObject); return; }
        SetColor();
    }
}
