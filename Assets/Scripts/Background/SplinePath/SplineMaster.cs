using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Background.SplinePath
{
    public class SplineMaster : BaseSplineBuilder
    {

        private const float ScaleFactorOfVelocityVectors = 0.5f;
        private float _oldScale;

        protected override void Start()
        {
            base.Start();
            _oldScale = ScaleFactorOfVelocityVectors;
        }

        protected override void Update()
        {
            base.Update();
            if (math.distance(_oldScale, ScaleFactorOfVelocityVectors) > 0.05f)
            {
                AssembleSpline();
                _oldScale = ScaleFactorOfVelocityVectors;
            }
        }

        public override void SetUpSplineSegment(int indexOfTheFirstPoint)
        {
            if (indexOfTheFirstPoint > splinePoints.Count - 2 || indexOfTheFirstPoint < 0)
            {
                return;
            }

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
            current.pointsForTheCurve = new[]
                { splinePoints[indexOfTheFirstPoint], splinePoints[indexOfTheFirstPoint + 1] };
            current.velocities = new[] { ScaleFactorOfVelocityVectors * v1, ScaleFactorOfVelocityVectors * v2 };
            current.gameObjectLineRenderer = LineRendererPrefab;
            current.mySplineBuilder = this;
            current.indexOfTheFirstPoint = indexOfTheFirstPoint;
            current.SubscripeToPointEvent();
            current.DeleteOldDraw();
            current.Draw();
        }

        protected override void AddPointToSpline(Transform newPoint)
        {
            splinePoints.Add(newPoint);
            splinePoints[splinePoints.Count-1].transform.SetParent(transform);
            SetUpSplineSegment(splinePoints.Count - 2);
            SetUpSplineSegment(splinePoints.Count - 3);
        }
    }

    public abstract class BaseSplineBuilder : MonoBehaviour
    {
        [SerializeField] protected List<Transform> splinePoints;
        [SerializeField] protected GameObject Point, LineRendererPrefab, DrawCurvePrefab;
        protected List<DrawCurve> DrawCurvesList = new List<DrawCurve>();
        protected Camera _camera;

        protected virtual void Start()
        {
            _camera = Camera.main;
        }

        protected virtual void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                Transform newPoint = Instantiate(Point, mousePosition, quaternion.identity).transform;
                AddPointToSpline(newPoint);
            }
        }

        [ContextMenu("AssembleSpline")] protected void AssembleSpline()
        {
            DeleteOldSpline();
            for (int i = 0; i < splinePoints.Count - 1; i++)
            {
                SetUpSplineSegment(i);
            }
        }
        public abstract void SetUpSplineSegment(int indexOfTheFirstPoint);

        protected void DeleteOldSpline()
        {
            foreach (DrawCurve drawCurve in DrawCurvesList)
            {
                drawCurve.DeleteOldDraw();
            }
        }

        protected abstract void AddPointToSpline(Transform newPoint);
    }
    
    public class BezierHermiteSpline : CurveBase
    {
        private static readonly Matrix4x4 MainMatrix = new Matrix4x4(new Vector4(1,0,0,0),new Vector4(0,1,0,0), new Vector4(-3,-2,3,-1),new Vector4(2,1,-2,1));
        // 1  0  0  0
        // 0  1  0  0
        //-3 -2  3 -1
        // 2  1 -2  1

        public BezierHermiteSpline(Transform point1,Vector3 velocity1,Transform point2,Vector3 velocity2)
        {
            Points = new Vector3[4];
            Points[0] = point1.position;
            Points[1] = velocity1;
            Points[2] = point2.position;
            Points[3] = velocity2;
        }

        public override Vector3 GetPoint(float t)
        {
            Vector4 pattern = new Vector4(1, t, math.pow(t, 2), math.pow(t, 3));

            Vector4 matrixFactoredPattern =  MainMatrix * pattern;

            Vector3 returnVector3 = Vector3.zero;

            returnVector3 += Points[0] * matrixFactoredPattern.x;
            returnVector3 += Points[1] * matrixFactoredPattern.y;
            returnVector3 += Points[2] * matrixFactoredPattern.z;
            returnVector3 += Points[3] * matrixFactoredPattern.w;
        
            return returnVector3;
        }
    }

    
    public abstract class CurveBase
    {
        protected Vector3[] Points;
        public abstract Vector3 GetPoint(float t);
    }
}
