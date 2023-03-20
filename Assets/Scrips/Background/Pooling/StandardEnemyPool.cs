namespace Scrips.Background.Pooling
{
    public class StandardEnemyPool : ObjectPooling
    {
        public static StandardEnemyPool Instance;

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
