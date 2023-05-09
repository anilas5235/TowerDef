using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Background.SplinePath
{
    [RequireComponent(typeof(LineRenderer))]
    public class DrawCurve : MonoBehaviour
    {
        [HideInInspector] public Transform[] pointsForTheCurve;
        [HideInInspector] public Vector3[] velocities;
        [HideInInspector] public BaseSplineBuilder mySplineBuilder;

        private List<GameObject> _drawnPoints = new List<GameObject>();
        private LineRenderer myLineRenderer;

        

        public void Draw()
        {
            if (myLineRenderer == null) { myLineRenderer = GetComponent<LineRenderer>(); }
            DeleteOldDraw();
            MatrixCurveBase matrixCurve;
            switch (mySplineBuilder.MySplineType)
            {
                case BaseSplineBuilder.SplineType.B_Spline:
                    matrixCurve = new BezierHermiteSpline(
                        pointsForTheCurve[0], velocities[0],
                        pointsForTheCurve[1], velocities[1]);
                    break;
                case BaseSplineBuilder.SplineType.Bezie_Spline:
                    matrixCurve = new BezierCurveMatrix(pointsForTheCurve[0], pointsForTheCurve[1],
                        pointsForTheCurve[2], pointsForTheCurve[3]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            List<Vector3> _points = new List<Vector3>();
            float t = 0;
            while (t < 1)
            {
                _points.Add(matrixCurve.GetPoint(t));
                t += 0.01f;
            }

            DrawPoints(_points.ToArray(), mySplineBuilder.LineColor, mySplineBuilder.LineThickness);
        }

        public void DeleteOldDraw()
        {
            if (myLineRenderer == null) { myLineRenderer = gameObject.GetComponent<LineRenderer>(); }
            if (mySplineBuilder.Tile == null) { myLineRenderer.positionCount = 0; }
        }

        private void DrawPoints(Vector3[] points, Color lineColor, float lineWidth)
        {
            int indexInDrawnPoints = 0;
            if (mySplineBuilder.Tile)
            {
                for (int i = 0; i < points.Length - 1;)
                {
                    int offset = 1;
                    while (Vector2.Distance(points[i], points[i + offset]) < mySplineBuilder.RESOLUTION)
                    {
                        if (i + offset >= points.Length - 1) break;
                        offset++;
                    }

                    if (indexInDrawnPoints > _drawnPoints.Count - 1)
                    {
                        _drawnPoints.Add(Instantiate(mySplineBuilder.Tile, transform.position, quaternion.identity));
                        _drawnPoints[_drawnPoints.Count - 1].name = $"LineDrawnBy{this.name}";
                    }

                    _drawnPoints[indexInDrawnPoints].gameObject.SetActive(true);
                    LineRenderer lineRenderer = _drawnPoints[indexInDrawnPoints].GetComponent<LineRenderer>();
                    lineRenderer.SetPositions(new[] { points[i], points[i + offset] });
                    lineRenderer.startColor = lineColor;
                    lineRenderer.endColor = lineColor;
                    lineRenderer.widthMultiplier = lineWidth;
                    lineRenderer.gameObject.transform.SetParent(gameObject.transform);
                    i += offset;
                    indexInDrawnPoints++;
                }
            }
            else
            {
                List<Vector3> pointsForLineRender = new List<Vector3>();
                pointsForLineRender.Add(points[0]);
                for (int i = 0; i < points.Length-1;)
                {
                    int offset = 1;
                    while (Vector2.Distance(points[i], points[i + offset]) < mySplineBuilder.RESOLUTION)
                    {
                        if (i + offset >= points.Length - 1)  break; 
                        offset++;
                    }

                    pointsForLineRender.Add(points[i + offset]);
                    i += offset;
                }
                pointsForLineRender.Add(points[points.Length-1]);

                myLineRenderer.positionCount = pointsForLineRender.Count;
                myLineRenderer.SetPositions(pointsForLineRender.ToArray());
                myLineRenderer.startColor = lineColor;
                myLineRenderer.endColor = lineColor;
                myLineRenderer.widthMultiplier = lineWidth;
                myLineRenderer.gameObject.transform.SetParent(gameObject.transform);
            }
        }

        private void DrawArms()
        {
            if (pointsForTheCurve.Length  < 1)return;
            Gizmos.color = Color.gray;
            Gizmos.DrawLine( pointsForTheCurve[0].position, pointsForTheCurve[1].position);
            Gizmos.DrawLine(pointsForTheCurve[2].position, pointsForTheCurve[3].position);
        }

        private void OnDrawGizmos()
        {
            DrawArms();
        }
    }
}

