using UnityEngine;
using Random = UnityEngine.Random;

namespace Scrips.Background.WaveManaging
{
    public class WaveCreator : MonoBehaviour
    {
        [SerializeField] private Wave targetWave;

        private void Start()
        {
            if (!targetWave)
            {
                return;
            }
            int points = 40;
            targetWave.SpawnData = new WavePoint[points];
            var D = new DiagonalDescendingPattern(2, 2, 4);
            var A = new ZigZackPattern(5, 3, 5);
            for (int i = 0; i < points; i++)
            {
                targetWave.SpawnData[i] = new WavePoint();
                targetWave.SpawnData[i].hp = new[] { D.GetValue(), A.GetValue()};
                targetWave.SpawnData[i].name = $"Step{i}";
            }
#if UNITY_EDITOR
             UnityEditor.EditorUtility.SetDirty(targetWave);
             UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

    }

    public abstract class PatternBase
    {
        protected int currentStep, min, max, repetitionCount;
        protected const int MinOutputValue = 1, MaxOutputValue = 10;

        protected PatternBase(int startValue, int lowerBound, int upperBound)
        {
            if (lowerBound < MinOutputValue) lowerBound = MinOutputValue;
            if (upperBound > MaxOutputValue) upperBound = MaxOutputValue;
            if (lowerBound > upperBound)
            {
                return;
            }

            if (startValue > upperBound) startValue = upperBound;
            if (startValue < lowerBound) startValue = lowerBound;
            min = lowerBound;
            max = upperBound;
        }

        protected PatternBase(int lowerBound, int upperBound)
        {
            if (lowerBound < MinOutputValue) lowerBound = MinOutputValue;
            if (upperBound > MaxOutputValue) upperBound = MaxOutputValue;
            if (lowerBound > upperBound) { return; }

            min = lowerBound;
            max = upperBound;
        }

        protected PatternBase(int startValue)
        {
            if (startValue > MaxOutputValue) startValue = MaxOutputValue;
            if (startValue < MinOutputValue) startValue = MinOutputValue;
           
        }

        protected PatternBase(int[] values)
        {
            if (values.Length < 1) { return; }
        }

        public virtual int GetValue()
        {
            var returnVal = Function(currentStep);
            if (returnVal > MaxOutputValue || returnVal < MinOutputValue || returnVal > max || returnVal < min)
            {
                currentStep = 0;
                repetitionCount++;
                returnVal = Function(currentStep);
            }

            currentStep++;
            return returnVal;
        }

        protected abstract int Function(int step);
    }

    public class DiagonalDescendingPattern : PatternBase
    {
        public DiagonalDescendingPattern(int startValue, int lowerBound, int upperBound) : base(startValue, lowerBound,
            upperBound)
        {
            currentStep = startValue - min;
        }

        protected override int Function(int step)
        {
            int returnVal = min;
            returnVal += currentStep;
            return returnVal;
        }
    }

    public class DiagonalRisingPattern : PatternBase
    {
        public DiagonalRisingPattern(int startVal, int lowerBound, int upperBound) : base(startVal, lowerBound,
            upperBound)
        {
            currentStep = max - startVal;
        }

        protected override int Function(int step)
        {
            int returnVal = max;
            returnVal -= currentStep;
            return returnVal;
        }
    }

    public class LinearPattern : PatternBase
    {
        private int _val;

        public LinearPattern(int startValue) : base(startValue)
        {
            _val = startValue;
        }

        protected override int Function(int step)
        {
            return _val;
        }
    }

    public class ZigZackPattern : PatternBase
    {
        private int patternLength;

        public ZigZackPattern(int startVal, int lowerBound, int upperBound) : base(startVal, lowerBound, upperBound)
        {
            currentStep = startVal - min;
            patternLength = max - min;
        }

        protected override int Function(int step)
        {
            int returnVal = 0;
            if (currentStep + 1 > patternLength)
            {
                currentStep += 1;
                return returnVal;
            }

            if (repetitionCount % 2 == 0)
            {
                returnVal = min;
                returnVal += currentStep;
            }
            else
            {
                returnVal = max;
                returnVal -= currentStep;
            }

            return returnVal;
        }
    }

    public class CustomPattern : PatternBase
    {
        private int[] customValues;

        public CustomPattern(int[] values) : base(values)
        {
            min = MinOutputValue;
            max = MaxOutputValue;
            customValues = values;
        }

        protected override int Function(int step)
        {
            if (step > customValues.Length - 1)
            {
                currentStep = 0;
                repetitionCount++;
                step = 0;
            }

            return customValues[step];
        }
    }

    public class RandomPattern : PatternBase
    {
        public RandomPattern(int min, int max) : base(min, max)
        {
        }

        protected override int Function(int step)
        {
            return Random.Range(min,max);
        }
    }
}
