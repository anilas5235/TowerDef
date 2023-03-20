using Scrips.Background;
using Scrips.Background.Pooling;
using UnityEngine;

namespace Scrips.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        private static StandardProjectilePool Pool;
            
        public Vector3 targetDirection;
        public float speed, currentScale, lifeTime;
        public int damage, pierce;
    
        [SerializeField] private SpriteRenderer mySpriteRenderer;
        [SerializeField] private LayerMask Enemy;
        private Collider2D[] cols = new Collider2D[50];

        private void Start()
        {
            if (Pool == null)
            {
                Pool = StandardProjectilePool.Instance;
            }
        }

        void Update()
        {
            transform.Translate(targetDirection * (speed * Time.deltaTime));
            if (Time.time > lifeTime && lifeTime > 0)
            {
                ResetProjectileValues();
                Pool.AddObjectToPool(gameObject);
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
                    Pool.AddObjectToPool(gameObject);
                }
            }
        }
        public void AppearanceUpdate()
        {
            currentScale = 0.1f + pierce / 10f;
            transform.localScale = new Vector3(currentScale, currentScale,1);
            mySpriteRenderer.color = ColorKeeper.StandardColors(damage-1);
        }

        private void OnBecameInvisible()
        {
            ResetProjectileValues();
            Pool.AddObjectToPool(gameObject);
        }

        public void ResetProjectileValues()
        {
            targetDirection = Vector3.zero;
            speed = 0;
            currentScale = 0;
            lifeTime = 0;
            damage = 0;
            pierce = 0;
        }
    }
}
