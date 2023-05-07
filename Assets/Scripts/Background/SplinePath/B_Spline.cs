using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Background.SplinePath
{
    public class B_Spline : BaseSplineBuilder
    {
        [SerializeField,Range(0.1f,1f)] private float ScaleFactorOfVelocityVectors = 0.5f;

        public void CheckForExistingComponents()
        {
            if (splinePoints.Count < 1)
            {
                foreach (var point in gameObject.GetComponentsInChildren<PointBehaviour>().ToList())
                {
                    splinePoints.Add(point.gameObject.transform);
                    point.Master = this;
                }
            }

            if (DrawCurvesList.Count < 1)
            {
                 DrawCurvesList = gameObject.GetComponentsInChildren<DrawCurve>().ToList();
            }
        }

        public override void SetUpSplineSegment(int indexOfTheFirstPoint)
        {
            foreach (Transform splinePoint in splinePoints)
            {
                if (splinePoint == null)
                {
                    AssembleSpline();
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
            current.velocities = new[] { ScaleFactorOfVelocityVectors * v1, ScaleFactorOfVelocityVectors * v2 };
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
            for (int i = 0; i < splinePoints.Count; i++)
            {
                if (splinePoints[i] == null)
                {
                    splinePoints.RemoveAt(i);
                }
                else
                {
                    splinePoints[i].gameObject.GetComponent<PointBehaviour>().index = i;
                }
            }
            if (splinePoints.Count < 3)return;
            base.AssembleSpline();
        }

        public override void TriggerPointMoved(int index)
        {
            for (int i = -2; i < 3; i++)
            {
                SetUpSplineSegment(index+ i);
            }
        }

        public void InitializeSpline()
        {
            LoadPrefabs();
            CheckForExistingComponents();
            while (splinePoints.Count < 3)
            {
                AddPointToSpline();
            }
        }
    }
}