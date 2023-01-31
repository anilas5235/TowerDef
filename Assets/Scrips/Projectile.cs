using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed, currentScale;
    public Vector3 targetDirection;
    public int damage, pierce;
    public Color projectileColor;
    
    [SerializeField] private SpriteRenderer mySpriteRenderer;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(targetDirection * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            col.gameObject.GetComponent<Enemy>().TakeDamage(damage);
            StatsKeeper.Instance.Money += damage; StatsKeeper.Instance.UpdateUI();
            pierce--;
           AppearanceUpdate();
        }
        else if(col.gameObject.CompareTag("DestoryProjectile")) { pierce -= pierce; }
        
        if(pierce < 1){Destroy(gameObject);}
    }

    public void AppearanceUpdate()
    {
        currentScale = 0.1f + pierce / 10f;
        transform.localScale = new Vector3(currentScale, currentScale,1);
        mySpriteRenderer.color = projectileColor;
    }
}
