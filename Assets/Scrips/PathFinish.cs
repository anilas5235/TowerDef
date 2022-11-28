using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinish : MonoBehaviour
{
    private StatsKeeper StatsKeeper;

    private void Awake()
    {
        StatsKeeper = GameObject.FindObjectOfType<StatsKeeper>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Enemy"))
        {
            StatsKeeper.hp -= col.gameObject.GetComponent<Enemy>().hp;
        }
        Destroy(col.gameObject);
    }
}
