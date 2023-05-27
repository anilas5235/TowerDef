namespace Background.WaveManaging
{
    [System.Serializable]
    public class WavePoint
    {
        public static int WavePointSize = 15;
        public WavePoint()
        {
            EnemyData = new int[WavePointSize];
            extraWait = 0;
        }
        public string Name;
        public int[] EnemyData;

        private int extraWait;
        public int ExtraWait
        {
            get => extraWait;
            set
            {
                if (value != extraWait)
                {
                    if (value < -1) value = -1;
                    extraWait = value;
                }
            }
        }
    }
}
