using System.Linq.Expressions;
using Background.Pooling;
using Background.WaveManaging;
using UnityEngine;

namespace Projectiles
{
    public class LavaShoot : MonoBehaviour
    {
        public int storedDamage;
        public bool pooled;
        
        [SerializeField] private SpriteRenderer mySpriteRenderer;
        [SerializeField] private LayerMask Enemy;
        [SerializeField] private ParticleSystem _particleSystem;
        private ParticleSystem.MainModule _mainModule;
        public Color[] Colors = new Color[5];
        
        private static LavaShootPool Pool;
        private Collider2D[] cols = new Collider2D[10];
        private float _detectRadius = 0.3f;

        private void Start()
        {
            Pool ??= LavaShootPool.Instance;
        }
        private void FixedUpdate()
        {
            if (pooled)return;
            
            if (SpawnManager.Instance.waveIsRunning == false)
            {
                pooled = true;
                Pool.AddObjectToPool(gameObject);
                return;
            }

            cols = new Collider2D[10];
            Physics2D.OverlapCircleNonAlloc(transform.position, _detectRadius, cols, Enemy);

            foreach (Collider2D col in cols)
            {
                if (col == null)
                { return; }
                if (col.gameObject.CompareTag("Enemy"))
                {
                    Enemy enemy = col.gameObject.GetComponent<Enemy>();
                    int enemyhp = enemy.hp;
                    enemy.TakeDamage(storedDamage);
                    storedDamage -= enemyhp;
                    if (storedDamage < 1)
                    {
                        pooled = true;
                        Pool.AddObjectToPool(gameObject);
                        return;
                    }
                    AppearanceUpdate();
                }
            }
        }
        public void AppearanceUpdate()
        {
            int colorIndex;

            if (storedDamage > 20) colorIndex = 4;
            else if (storedDamage > 15) colorIndex = 3;
            else if (storedDamage > 10) colorIndex = 2;
            else if (storedDamage > 5) colorIndex = 1;
            else colorIndex = 0;
            
            mySpriteRenderer.color = Colors[colorIndex];
            _mainModule = _particleSystem.main;
            _mainModule.startColor = colorIndex < Colors.Length-1 ? Colors[colorIndex+1]: Color.white;
        }
    }
}
