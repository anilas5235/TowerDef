using System;
using Background.Pooling;
using Background.WaveManaging;
using Scrips.Background;
using UnityEngine;

namespace Scrips.Projectiles
{
    public class LavaShoot : MonoBehaviour
    {
        public int storedDamage;
        public bool pooled;
        
        [SerializeField] private SpriteRenderer mySpriteRenderer;
        [SerializeField] private LayerMask Enemy;
        public Color[] Colors = new Color[5];
        
        private static LavaShootPool Pool;
        private Collider2D[] cols = new Collider2D[10];
        private float _detectRadius = 0.3f;

        private void Start()
        {
            if (Pool == null)
            {
                Pool = LavaShootPool.Instance;
            }
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
            Color newColor;
            
            if (storedDamage > 20) { newColor = Colors[4]; }
            else if (storedDamage > 15) { newColor = Colors[3]; }
            else if (storedDamage > 10) { newColor = Colors[2]; }
            else if (storedDamage > 5) { newColor = Colors[1]; }
            else  { newColor = Colors[0]; }
            mySpriteRenderer.color = newColor;
        }
    }
}
