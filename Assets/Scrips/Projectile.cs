using UnityEngine;

namespace Scrips
{
    public class Projectile : MonoBehaviour
    {
        public Vector3 targetDirection;
        public Color projectileColor;
        public float speed, currentScale, lifeTime;
        public int damage, pierce;
    
        [SerializeField] private SpriteRenderer mySpriteRenderer;

        void Update()
        {
            transform.Translate(targetDirection * (speed * Time.deltaTime));
            if (Time.time > lifeTime && lifeTime > 0)
            { Destroy(gameObject); }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Enemy"))
            {
                col.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                pierce--;
                AppearanceUpdate();
            }
            else if(col.gameObject.CompareTag("DestoryProjectile") || col.gameObject.CompareTag("Tower")) { pierce -= pierce; }
        
            if(pierce < 1){Destroy(gameObject);}
        }

        public void AppearanceUpdate()
        {
            currentScale = 0.1f + pierce / 10f;
            transform.localScale = new Vector3(currentScale, currentScale,1);
            mySpriteRenderer.color = projectileColor;
        }
    }
}
