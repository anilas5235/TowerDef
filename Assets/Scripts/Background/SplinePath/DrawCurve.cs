using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
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

            switch (mySplineBuilder.CurrentDrawMode)
            {
                case BaseSplineBuilder.DrawMode.LineRender: DrawPointsWithLineRender(_points.ToArray(), mySplineBuilder.LineColor, mySplineBuilder.LineThickness);
                    break;
                case BaseSplineBuilder.DrawMode.ObjectTiling: DrawPointsWithTiles(_points.ToArray(),mySplineBuilder.Tile,mySplineBuilder.tileSizeMultiplier);
                    break;
            }

            
        }

        public void DeleteOldDraw()
        {
            if (myLineRenderer == null) { myLineRenderer = gameObject.GetComponent<LineRenderer>(); }
            if (mySplineBuilder.Tile == null) { myLineRenderer.positionCount = 0; }
        }

        private void DrawPointsWithLineRender(Vector3[] points, Color lineColor, float lineWidth)
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

        private void DrawPointsWithTiles(Vector3[] points, GameObject tile, float sizeMultiplier)
        {
            int indexInDrawnPoints = 0;
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
                    _drawnPoints.Add(Instantiate(tile, transform.position, quaternion.identity));
                    _drawnPoints[_drawnPoints.Count - 1].name = $"TileDrawnBy{this.name}";
                }

                _drawnPoints[indexInDrawnPoints].gameObject.SetActive(true);
                GameObject current = _drawnPoints[indexInDrawnPoints];
                current.transform.localScale *= sizeMultiplier;
                current.gameObject.transform.SetParent(gameObject.transform);
                i += offset;
                indexInDrawnPoints++;
            }
        }
        private void DrawArms()
        {
            Gizmos.color = Color.gray;
            DrawLineGizmoLine(pointsForTheCurve[0].position, pointsForTheCurve[1].position,5f);
            DrawLineGizmoLine(pointsForTheCurve[2].position, pointsForTheCurve[3].position,5f);
        }

        private static void DrawLineGizmoLine(Vector3 p1, Vector3 p2, float width)
        {
            int count = 1 + Mathf.CeilToInt(width); // how many lines are needed.
            if (count == 1)
            {
                Gizmos.DrawLine(p1, p2);
            }
            else
            {
                Camera c = Camera.current;
                if (c == null)
                {
                    Debug.LogError("Camera.current is null");
                    return;
                }
                var scp1 = c.WorldToScreenPoint(p1);
                var scp2 = c.WorldToScreenPoint(p2);
 
                Vector3 v1 = (scp2 - scp1).normalized; // line direction
                Vector3 n = Vector3.Cross(v1, Vector3.forward); // normal vector
 
                for (int i = 0; i < count; i++)
                {
                    Vector3 o = 0.99f * n * width * ((float)i / (count - 1) - 0.5f);
                    Vector3 origin = c.ScreenToWorldPoint(scp1 + o);
                    Vector3 destiny = c.ScreenToWorldPoint(scp2 + o);
                    Gizmos.DrawLine(origin, destiny);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (CheckForDeletedObjects())return;
            DrawArms();
        }

        private bool CheckForDeletedObjects()
        {
            if (pointsForTheCurve.Length  < 4)return true;
            bool nullDetected = false;
            for (int i = pointsForTheCurve.Length-1; i > 0; i--)
            {
                if (nullDetected)
                {
                    if (pointsForTheCurve[i] != null) DestroyImmediate(pointsForTheCurve[i].gameObject);
                }
                else
                {
                    if (pointsForTheCurve[i] == null)
                    {
                        nullDetected = true;
                        i = pointsForTheCurve.Length;
                    }
                }
            }
            if (nullDetected)
            {
                DestroyImmediate(gameObject);
                mySplineBuilder.AssembleSpline();
            }
            return nullDetected;
        }
    }
}

