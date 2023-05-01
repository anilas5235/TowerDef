using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
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
                splinePoints = new List<Transform>();
                foreach (var point in gameObject.GetComponentsInChildren<PointBehaviour>().ToList())
                {
                    splinePoints.Add(Point.gameObject.transform);
                }
            }

            if (DrawCurvesList.Count < 1)
            {
                DrawCurvesList = gameObject.GetComponentsInChildren<DrawCurve>().ToList();
            }
        }

        public override void SetUpSplineSegment(int indexOfTheFirstPoint)
        {
            if (splinePoints.Count < 3)
            {
                Debug.Log("this Spline needs at least three Points");
                return;
            }
            if (indexOfTheFirstPoint > splinePoints.Count - 2 || indexOfTheFirstPoint < 0) return;

            Vector3 v1 = Vector3.zero, v2 = Vector3.zero;
            if (indexOfTheFirstPoint < 1)
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
                DrawCurvesList.Add(Instantiate(DrawCurvePrefab, transform.position, quaternion.identity).gameObject
                    .GetComponent<DrawCurve>());
                DrawCurvesList[DrawCurvesList.Count-1].gameObject.name = $"Segment{DrawCurvesList.Count - 1}";
            }

            DrawCurve current = DrawCurvesList[indexOfTheFirstPoint];
            current.gameObject.transform.SetParent(transform);
            current.pointsForTheCurve = new[] { splinePoints[indexOfTheFirstPoint], splinePoints[indexOfTheFirstPoint + 1] };
            current.velocities = new[] { ScaleFactorOfVelocityVectors * v1, ScaleFactorOfVelocityVectors * v2 };
            current.mySplineBuilder = this;
            current.indexOfTheFirstPoint = indexOfTheFirstPoint;
            current.SubscripeToPointEvent();
            current.Draw();
        }

        public override void AddPointToSpline()
        {
            Transform newPoint = Instantiate(Point, transform.position, quaternion.identity).transform;
            splinePoints.Add(newPoint);
            splinePoints[splinePoints.Count - 1].transform.SetParent(transform);
            if (splinePoints.Count < 3)return;
            SetUpSplineSegment(splinePoints.Count - 2);
            SetUpSplineSegment(splinePoints.Count - 3);
        }

        public override void AssembleSpline()
        {
            if (splinePoints.Count < 3)return;
            base.AssembleSpline();
        }
    }
}