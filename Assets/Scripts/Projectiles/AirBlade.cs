using System;
using System.Collections;
using System.Collections.Generic;
using Background.Pooling;
using UnityEngine;

namespace Projectiles
{
    public class AirBlade : MonoBehaviour
    {
        public Vector3 direction;
        public float speed;
        public float sizeMultiplier = 0.3f;

        [SerializeField] private Transform parent;
        private SpriteRenderer mySpriteRenderer;
        private float sizeOverTimeMultiplier = 0.1f, lifeTime =1f;
        private List<Transform> attachedEnemies = new List<Transform>();
        private Color originalColor;

        private void OnEnable()
        {
            if (mySpriteRenderer) return;
            mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            originalColor = mySpriteRenderer.color;
        }

        void Update()
        {
            parent.position += direction * (Time.deltaTime * speed);
            transform.localScale = Vector3.one * (sizeOverTimeMultiplier * sizeMultiplier);
            mySpriteRenderer.color = new Color(originalColor.r,originalColor.g,originalColor.b, originalColor.r - ((sizeOverTimeMultiplier / lifeTime)*0.2f));
            if (sizeOverTimeMultiplier < lifeTime) sizeOverTimeMultiplier += Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Enemy"))
            {
                if (attachedEnemies.Contains(col.transform))return;
                col.transform.SetParent(parent);
                attachedEnemies.Add(col.transform);
                col.gameObject.GetComponent<Enemy>().StartDrift();
            }
        }

        private void OnDestroy() => AppendEnemies();

        private void AppendEnemies()
        {
            foreach (Transform attachedEnemy in attachedEnemies)
            {
                if (!attachedEnemy) continue;
                attachedEnemy.transform.SetParent(null);
                Enemy enemy = attachedEnemy.gameObject.GetComponent<Enemy>();
                enemy.StopDrift();
            }
            attachedEnemies = new List<Transform>();
        }

        public void StartDeathTimer(float time)
        {
            lifeTime = time;
            StartCoroutine( DeathTimer(lifeTime));
        }

        private IEnumerator DeathTimer(float time)
        {
            yield return new WaitForSeconds(time);
            AppendEnemies();
            mySpriteRenderer.color = originalColor;
            sizeOverTimeMultiplier = 0.1f;
            transform.localScale = Vector3.one * 0.3f;
            AirBladePool.Instance.AddObjectToPool(parent.gameObject);
        }
    }
}
