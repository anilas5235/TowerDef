using Scrips.Projectiles;
using UnityEngine;

namespace Scrips.Background
{
    public class StandardProjectilePool : ObjectPooling
    {
        public static StandardProjectilePool Instance;

        protected override void Start()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            base.Start();
        }

        protected override void UpdateName()
        {
            gameObject.name = $"{objectToPool.name}Pool({transform.childCount})";
        }

        protected override void SetPoolingReference(GameObject objectRef)
        {
            objectRef.GetComponent<Projectile>().pool = this;
        }
    }
}
