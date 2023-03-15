using Scrips.Background;
using UnityEngine;

namespace Scrips.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public StandardProjectilePool pool;
            
        public Vector3 targetDirection;
        public Color projectileColor;
        public float speed, currentScale, lifeTime;
        public int damage, pierce;
    
        [SerializeField] private SpriteRenderer mySpriteRenderer;
        [SerializeField] private LayerMask Enemy;
        private Collider2D[] cols = new Collider2D[50];
        void Update()
        {
            transform.Translate(targetDirection * (speed * Time.deltaTime));
            if (Time.time > lifeTime && lifeTime > 0)
            {
                ResetProjectileValues();
                pool.AddObjectToPool(gameObject);
            }
        }

        private void FixedUpdate()
        {
            Physics2D.OverlapCircleNonAlloc(transform.position, transform.localScale.x/2f, cols, Enemy);

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
                    pool.AddObjectToPool(gameObject);
                }
            }
        }
        public void AppearanceUpdate()
        {
            currentScale = 0.1f + pierce / 10f;
            transform.localScale = new Vector3(currentScale, currentScale,1);
            mySpriteRenderer.color = projectileColor;
        }

        private void OnBecameInvisible()
        {
            ResetProjectileValues();
            pool.AddObjectToPool(gameObject);
        }

        public void ResetProjectileValues()
        {
            targetDirection = Vector3.zero;
            projectileColor = Color.white;
            speed = 0;
            currentScale = 0;
            lifeTime = 0;
            damage = 0;
            pierce = 0;
        }
    }
}
