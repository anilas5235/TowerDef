using UnityEditor;
using UnityEngine;
using Unity.Mathematics;

namespace Background.SplinePath
{
    public class B_Spline : BaseSplineBuilder
    {
        [SerializeField,Range(0.1f,1f)] private float TentionScale = 0.5f;
        private float currentTentionScale = default;

        public override void SetUpSplineSegment(int indexOfTheFirstPoint)
        {
            MySplineType = SplineType.B_Spline;
            foreach (Transform splinePoint in splinePoints)
            {
                if (splinePoint == null)
                {
                    this.AssembleSpline();
                    return;
                }
            }
            if (splinePoints.Count < 3)return;
            
            if (indexOfTheFirstPoint > splinePoints.Count - 2 || indexOfTheFirstPoint < 0) return;

            Vector3 v1 = Vector3.zero, v2 = Vector3.zero;
            if (indexOfTheFirstPoint == 0)
            {
                Vector3 ghostPoint = splinePoints[0].position +
                                     -1 * (splinePoints[0].position - splinePoints[1].position);
                v1 = splinePoints[1].position - ghostPoint;
                v2 = splinePoints[2].position - splinePoints[0].position;

            }
            else if (indexOfTheFirstPoint < splinePoints.Count - 2)
            {
                v1 = splinePoints[indexOfTheFirstPoint + 1].position - splinePoints[indexOfTheFirstPoint - 1].position;
                v2 = splinePoints[indexOfTheFirstPoint + 2].position - splinePoints[indexOfTheFirstPoint].position;
            }
            else
            {
                Vector3 ghostPoint = splinePoints[indexOfTheFirstPoint + 1].position + -1 *
                    (splinePoints[indexOfTheFirstPoint + 1].position - splinePoints[indexOfTheFirstPoint].position);
                v1 = splinePoints[indexOfTheFirstPoint + 1].position - splinePoints[indexOfTheFirstPoint - 1].position;
                v2 = ghostPoint - splinePoints[indexOfTheFirstPoint].position;
            }

            if (indexOfTheFirstPoint > DrawCurvesList.Count - 1)
            {
                DrawCurvesList.Add( ((GameObject)PrefabUtility.InstantiatePrefab(DrawCurvePrefab)).GetComponent<DrawCurve>());
                DrawCurvesList[DrawCurvesList.Count-1].gameObject.name = $"Segment{DrawCurvesList.Count - 1}";
            }

            if (indexOfTheFirstPoint > DrawCurvesList.Count-1)return;
            DrawCurve current = DrawCurvesList[indexOfTheFirstPoint];
            current.gameObject.transform.SetParent(transform);
            current.pointsForTheCurve = new[] { splinePoints[indexOfTheFirstPoint], splinePoints[indexOfTheFirstPoint + 1] };
            current.velocities = new[] { TentionScale * v1, TentionScale * v2 };
            current.mySplineBuilder = this;
            current.Draw();
        }

        protected override void UpdatePointsAdded()
        {
            if (splinePoints.Count < 3) return;
            SetUpSplineSegment(splinePoints.Count - 2);
            SetUpSplineSegment(splinePoints.Count - 3);
        }

        public override void AssembleSpline()
        {
            MySplineType = SplineType.B_Spline;
            if (splinePoints.Count < 3)return;
            base.AssembleSpline();
            for (int i = 0; i < splinePoints.Count - 1; i++) SetUpSplineSegment(i);
        }

        public override void TriggerPointMoved(int index)
        {
            if (splinePoints.Count < 1 || DrawCurvesList.Count <1) CheckForExistingComponents();
            for (int i = -2; i < 3; i++)
            {
                SetUpSplineSegment(index+ i);
            }
        }

        public override void InitializeSpline()
        {
            LoadPrefabs();
            while (splinePoints.Count < 3)
            {
                AddPointToSpline();
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            if (math.abs( currentTentionScale- TentionScale) > 0.009f)
            {
                currentTentionScale = TentionScale;
                AssembleSpline();
            }
        }
    }
}