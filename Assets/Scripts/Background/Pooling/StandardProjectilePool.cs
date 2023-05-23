
using System;

namespace Background.Pooling
{
    public class StandardProjectilePool : ObjectPooling
    {
        public static StandardProjectilePool Instance;

        private void Awake()
        {
            if (!Instance) Instance = this;
            else Destroy(gameObject);
        }
    }
}
