
namespace Background.Pooling
{
    public class LavaShootPool : ObjectPooling
    {
        
        public static LavaShootPool Instance;

        private void Awake()
        {
            if (!Instance) Instance = this;
            else Destroy(gameObject);
        }
    }
}
