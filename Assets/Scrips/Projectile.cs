using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public Vector3 targetDirection;
    public int damage, pierce;
    public Color projectileColor;
    private bool coloerSet = false;
    [SerializeField] private SpriteRenderer mySpriteRenderer;

  
    // Update is called once per frame
    void Update()
    {
        if(!coloerSet){mySpriteRenderer.color = projectileColor; coloerSet = true; }
        transform.Translate(targetDirection * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            col.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            StatsKeeper.Instance.Money += damage; StatsKeeper.Instance.UpdateUI();
            pierce--;
        }
        else if(col.gameObject.CompareTag("DestoryProjectile")) { pierce -= pierce; }
        
        if(pierce < 1){Destroy(gameObject);}
    }
}
