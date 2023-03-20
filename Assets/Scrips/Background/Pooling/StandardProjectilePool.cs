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
        }
    }
}
