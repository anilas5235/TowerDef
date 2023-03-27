using UnityEngine;

namespace Scrips.Background.WaveManaging
{
    public class WaveCreator : MonoBehaviour
    {
        
    }

    public abstract class PatternBase
    {
        protected PatternBase(int startVal)
        {
            startValue = startVal;
        }

        protected int startValue;
        private int currentStep, patternLength;

        public int GetValue()
        {
            int returnVal = Function(currentStep);
            currentStep++;
            if (currentStep > patternLength)
            {
                currentStep = 0;
            }
            return returnVal;
        }

        protected abstract int Function(int step);
    }

    public class ZigZackPattern : PatternBase
    {
        public ZigZackPattern(int startVal) : base(startVal){}
        protected override int Function(int step)
        {
            int returnVal = startValue;
            switch (step)
            {
                case 0: break;
                case 1: returnVal++; break;
            }

            return returnVal;
        }
    }
}
