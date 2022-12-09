using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public Vector3 targetDirection;
    public int damage, pierce;

    // Update is called once per frame
    void Update()
    {
        transform.position += targetDirection * (speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            col.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            StatsKeeper.Instance.Money += damage;
            pierce--;
        }
        else if(col.gameObject.CompareTag("DestoryProjectile")) { pierce -= pierce; }
        
        if(pierce < 1){Destroy(gameObject);}
    }
}
