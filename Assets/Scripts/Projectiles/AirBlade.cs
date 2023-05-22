using System;
using System.Collections.Generic;
using Scrips;
using UnityEngine;

namespace Projectiles
{
    public class AirBlade : MonoBehaviour
    {
        public Vector3 direction;
        public float speed;

        private float sizeMultiplier = 0.1f;
        private List<Transform> attachedEnemies = new List<Transform>();
        private Transform parent;

        //[SerializeField] private Collider2D myCollider;
        //[SerializeField] private LayerMask enemyLayer;
        //private ContactFilter2D EnemyFilter;

        // private void OnEnable()
        // {
        //     EnemyFilter.NoFilter();
        //     EnemyFilter.SetLayerMask(enemyLayer);
        //     EnemyFilter.SetDepth(-50, 50);
        // }

        private void OnEnable()
        {
            parent = transform.parent;
        }

        void Update()
        {
            parent.position += direction * (Time.deltaTime * speed);
            transform.localScale = Vector3.one * sizeMultiplier;
            if (sizeMultiplier < .3f) sizeMultiplier += Time.deltaTime;
            /*
            List<Collider2D> cols = new List<Collider2D>();
            myCollider.OverlapCollider(EnemyFilter, cols);

            foreach (Collider2D col in cols)
            {
                col.GetComponent<Enemy>().ThrowBack();
            }
            */
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Enemy"))
            {
                if (attachedEnemies.Contains(col.transform))return;
                col.transform.SetParent(parent);
                attachedEnemies.Add(col.transform);
                col.gameObject.GetComponent<Enemy>().Freeze();
            }
        }

        private void OnDestroy()
        {
            foreach (Transform attachedEnemy in attachedEnemies)
            {
                attachedEnemy.transform.SetParent(null);
                Enemy enemy = attachedEnemy.gameObject.GetComponent<Enemy>();
                enemy.UnFreeze();
                enemy.SetBackInPath(1);
            }
        }

        public void KillYourSelf(float lifeTime)
        {
            Destroy(gameObject, lifeTime);
        }
    }
}
