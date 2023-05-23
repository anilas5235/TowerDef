using System;
using Background.Keeper;
using Background.Pooling;
using Scrips.Background;
using UnityEngine;

namespace Scrips.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        private static StandardProjectilePool Pool;
            
        public Vector3 targetDirection;
        public float speed, currentScale, lifeTime;
        public int damage, pierce;
        public bool pooled;
    
        [SerializeField] private SpriteRenderer mySpriteRenderer;
        [SerializeField] private LayerMask Enemy, Blocking;
        private Collider2D[] cols;

        private void Start()
        {
            if (Pool == null)
            {
                Pool = StandardProjectilePool.Instance;
            }
        }

        void Update()
        {
            if (pooled ) return;
            transform.Translate(targetDirection * (speed * Time.deltaTime));
            if (Time.time > lifeTime && lifeTime > 0)
            {
                ResetProjectileValues();
                pooled = true;
                Pool.AddObjectToPool(gameObject);
                return;
            }
        }

        private void FixedUpdate()
        {
            if (pooled ) return;

            cols = new Collider2D[50];
            
            Physics2D.OverlapCircleNonAlloc(transform.position, transform.localScale.x/2f, cols, Enemy + Blocking);

            foreach (Collider2D col in cols)
            {
                if (col == null)
                { return; }
                if (col.gameObject.CompareTag("Enemy"))
                {
                    col.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                    pierce--;
                    AppearanceUpdate();
                }
                else if (col.gameObject.CompareTag("DestoryProjectile"))
                {
                    pierce -= pierce;
                }

                if (pierce < 1)
                {
                    ResetProjectileValues();
                    pooled = true;
                    Pool.AddObjectToPool(gameObject);
                    return;
                }
            }
        }
        public void AppearanceUpdate()
        {
            currentScale = 0.1f + pierce / 10f;
            transform.localScale = new Vector3(currentScale, currentScale,1);
            mySpriteRenderer.color = ColorKeeper.StandardColors(damage-1);
        }
        
        public void ResetProjectileValues()
        {
            cols = new Collider2D[50];
            targetDirection = Vector3.zero;
            speed = 0;
            currentScale = 0;
            lifeTime = 0;
            damage = 0;
            pierce = 1;
        }
    }
}
