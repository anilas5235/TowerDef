using Scrips.Projectiles;
using UnityEngine;

namespace Scrips.Background
{
    public class LavaShootPool : ObjectPooling
    {
        
        public static LavaShootPool Instance;

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
            objectRef.GetComponent<LavaShoot>().pool = this;
        }
    }
}
