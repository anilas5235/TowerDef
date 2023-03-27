using Unity.Mathematics;

namespace Scrips.Background.Pooling
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

            for (int i = 0; i < 10; i++)
            {
                AddObjectToPool(Instantiate(objectToPool,transform.position,quaternion.identity));
            }
        }
    }
}
