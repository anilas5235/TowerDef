
namespace Scrips.Background.Pooling
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
    }
}
