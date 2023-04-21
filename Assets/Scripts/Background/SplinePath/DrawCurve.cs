using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Background.SplinePath
{


    public class DrawCurve : MonoBehaviour
    {
        public Transform[] pointsForTheCurve;
        public GameObject gameObjectLineRenderer;
        public Vector3[] velocities;
        public BaseSplineBuilder mySplineBuilder;
        public int indexOfTheFirstPoint;

        private List<GameObject> _drawnPoints = new List<GameObject>();
        private bool _subedToPoints;
        [SerializeField, Range(0.01f, 1f)] private float RESOLUTION = 0.2f;

        private void OnEnable()
        {
            SubscripeToPointEvent();
        }

        private void Start()
        {
            Draw();
        }

        private void OnDisable()
        {
            foreach (Transform point in pointsForTheCurve)
            {
                if (point)
                {
                    point.gameObject.GetComponent<PointBehaviour>().PointMoved -= Draw;
                }
            }

            _subedToPoints = false;
        }

        public void Draw()
        {
            DeleteOldDraw();
            CurveBase curve = new BezierHermiteSpline(
                pointsForTheCurve[0], velocities[0],
                pointsForTheCurve[1], velocities[1]);



            List<Vector3> _points = new List<Vector3>();
            float t = 0;
            while (t < 1)
            {
                _points.Add(curve.GetPoint(t));
                t += 0.01f;
            }

            DrawPoints(_points.ToArray(), Color.white, 0.5f);
        }

        public void DeleteOldDraw()
        {
            if (_drawnPoints.Count > 0)
            {
                for (int i = 0; i < _drawnPoints.Count; i++)
                {
                    _drawnPoints[i].gameObject.SetActive(false);
                }
            }
        }

        public void DrawPoints(Vector3[] points, Color lineColor, float lineWidth)
        {
            int indexInDarwenPoints = 0;
            for (int i = 0; i < points.Length - 1;)
            {
                int offset = 1;
                while (Vector2.Distance(points[i], points[i + offset]) < RESOLUTION)
                {
                    if (i + offset >= points.Length - 1)
                    {
                        break;
                    }

                    offset++;
                }

                if (indexInDarwenPoints > _drawnPoints.Count - 1)
                {
                    _drawnPoints.Add(Instantiate(gameObjectLineRenderer, transform.position, quaternion.identity));
                    _drawnPoints[_drawnPoints.Count - 1].name = $"LineDrawnBy{this.name}";
                }

                _drawnPoints[indexInDarwenPoints].gameObject.SetActive(true);
                LineRenderer lineRenderer = _drawnPoints[indexInDarwenPoints].GetComponent<LineRenderer>();
                lineRenderer.SetPositions(new[] { points[i], points[i + offset] });
                lineRenderer.startColor = lineColor;
                lineRenderer.endColor = lineColor;
                lineRenderer.widthMultiplier = lineWidth;
                lineRenderer.gameObject.transform.SetParent(gameObject.transform);
                i += offset;
                indexInDarwenPoints++;
            }
        }

        public void DrawArms()
        {
            DrawPoints(new[] { pointsForTheCurve[0].position, pointsForTheCurve[1].position }, Color.gray, 0.3f);
            DrawPoints(new[] { pointsForTheCurve[2].position, pointsForTheCurve[3].position }, Color.gray, 0.3f);
        }

        private void TriggerPointMoved()
        {
            if (mySplineBuilder != null)
            {
                mySplineBuilder.SetUpSplineSegment(indexOfTheFirstPoint);
                mySplineBuilder.SetUpSplineSegment(indexOfTheFirstPoint + 1);
                mySplineBuilder.SetUpSplineSegment(indexOfTheFirstPoint - 1);
            }
            else
            {
                Draw();
            }
        }

        public void SubscripeToPointEvent()
        {
            if (_subedToPoints || pointsForTheCurve.Length < 1)
            {
                return;
            }

            foreach (Transform point in pointsForTheCurve)
            {
                if (point)
                {
                    point.gameObject.GetComponent<PointBehaviour>().PointMoved += TriggerPointMoved;
                }
            }

            _subedToPoints = true;
        }
    }
}

