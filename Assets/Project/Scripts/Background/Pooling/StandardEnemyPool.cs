namespace Background.Pooling
{
    public class StandardEnemyPool : ObjectPooling
    {
        public static StandardEnemyPool Instance;

        private void Awake()
        {
            if (!Instance) Instance = this;
            else Destroy(gameObject);
        }
    }
}
