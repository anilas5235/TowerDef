using Random = UnityEngine.Random;

namespace Background.WaveManaging
{
    public abstract class PatternBase
    {
        protected int currentStep, min, max, repetitionCount;
        protected const int MinOutputValue = 0, MaxOutputValue = 15;
        
        public enum PatternTypes
        {
            DiagonalDescending = 0,
            DiagonalRising =1,
            Linear =2,
            ZigZack =3,
            Custom =4,
            Random = 5,
        }
        

        protected PatternBase(int startValue, int lowerBound, int upperBound)
        {
            if (lowerBound < MinOutputValue) lowerBound = MinOutputValue;
            if (upperBound > MaxOutputValue) upperBound = MaxOutputValue;
            if (lowerBound > upperBound) return;

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
            if (values.Length < 1) return; 
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
        public DiagonalDescendingPattern(int startValue, int lowerBound, int upperBound) :
            base(startValue, lowerBound, upperBound)
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
        public DiagonalRisingPattern(int startVal, int lowerBound, int upperBound) :
            base(startVal, lowerBound, upperBound)
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
        private int[] _customValues;

        public CustomPattern(int[] values) : base(values)
        {
            min = MinOutputValue;
            max = MaxOutputValue;
            _customValues = values;
        }

        protected override int Function(int step)
        {
            if (step > _customValues.Length - 1)
            {
                currentStep = 0;
                repetitionCount++;
                step = 0;
            }

            return _customValues[step];
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
