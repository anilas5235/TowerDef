namespace Background.Pooling
{
    public class AirBladePool : ObjectPooling
    {
        public static AirBladePool Instance;

        protected override void Start()
        {
            if (!Instance) Instance = this;
            else Destroy(gameObject);

            base.Start();
        }
    }
}