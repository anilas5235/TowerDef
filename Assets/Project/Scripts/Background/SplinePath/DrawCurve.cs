using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Background.SplinePath
{
    [RequireComponent(typeof(LineRenderer))]
    public class DrawCurve : MonoBehaviour
    {
        public Transform[] pointsForTheCurve;
        [HideInInspector] public Vector3[] velocities;
        [HideInInspector] public BaseSplineBuilder mySplineBuilder;
        [HideInInspector] public List< Vector3> usedPoints = new List<Vector3>();
        [SerializeField] private SplineCollider2D mySplineCollider2D;

        private List<GameObject> _drawnObjects = new List<GameObject>();
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
                case BaseSplineBuilder.DrawMode.LineRender:
                    DrawPointsWithLineRender(_points.ToArray(), mySplineBuilder.LineColor,
                        mySplineBuilder.LineThickness);
                    myLineRenderer.Simplify(0.01f);
                    if (mySplineCollider2D) mySplineCollider2D.CreateCollider();
                    Vector3[] lineRendererPoints = new Vector3[myLineRenderer.positionCount];
                    myLineRenderer.GetPositions(lineRendererPoints);
                    usedPoints = lineRendererPoints.ToList();
                    break;
                case BaseSplineBuilder.DrawMode.ObjectTiling:
                    DrawPointsWithTiles(_points.ToArray(), mySplineBuilder.Tile, mySplineBuilder.tileSizeMultiplier);
                    break;
            }
        }

        public void DeleteOldDraw()
        {
            if (myLineRenderer == null) { myLineRenderer = gameObject.GetComponent<LineRenderer>(); }
            
            switch (mySplineBuilder.CurrentDrawMode)
            {
                case BaseSplineBuilder.DrawMode.LineRender:
                    myLineRenderer.enabled = true;
                    if (mySplineBuilder.Tile == null) { myLineRenderer.positionCount = 0; }
                    DeleteMyTiles();
                    break;
                case BaseSplineBuilder.DrawMode.ObjectTiling:
                    foreach (GameObject drawnPoint in _drawnObjects) drawnPoint.SetActive(false);
                    myLineRenderer.enabled = false;
                    break;
            }
        }

        private void DrawPointsWithLineRender(Vector3[] points, Color lineColor, float lineWidth)
        {
            usedPoints = new List<Vector3>();
            usedPoints.Add(points[0]);
            for (int i = 0; i < points.Length-1;)
            {
                int offset = 1;
                while (Vector2.Distance(points[i], points[i + offset]) < mySplineBuilder.RESOLUTION)
                {
                    if (i + offset >= points.Length - 1)  break; 
                    offset++;
                }

                usedPoints.Add(points[i + offset]);
                i += offset;
            }
            usedPoints.Add(points[points.Length-1]);

            myLineRenderer.positionCount = usedPoints.Count;
            myLineRenderer.SetPositions(usedPoints.ToArray());
            myLineRenderer.startColor = lineColor;
            myLineRenderer.endColor = lineColor;
            myLineRenderer.widthMultiplier = lineWidth;
            myLineRenderer.gameObject.transform.SetParent(gameObject.transform);
        }

        private void DrawPointsWithTiles(Vector3[] points, GameObject tile, float sizeMultiplier)
        {
            if (mySplineBuilder.Tile == null) return;
            if (_drawnObjects.Count < 1)
            {
                for (int i = 0; i < transform.childCount - 1; i++)
                {
                    _drawnObjects.Add(transform.GetChild(i).gameObject);
                }
            }

            usedPoints = new List<Vector3>();
            usedPoints.Add(points[0]);

            int indexInDrawnPoints = 0;
            for (int i = 0; i < points.Length - 1;)
            {
                int offset = 1;
                while (Vector2.Distance(points[i], points[i + offset]) < mySplineBuilder.RESOLUTION)
                {
                    if (i + offset >= points.Length - 1) break;
                    offset++;
                }
                usedPoints.Add(points[i + offset]);

                if (indexInDrawnPoints > _drawnObjects.Count - 1)
                {
                    _drawnObjects.Add(Instantiate(tile, transform.position, quaternion.identity));
                    _drawnObjects[_drawnObjects.Count - 1].name = $"TileDrawnBy{this.name}";
                }

                if (_drawnObjects[indexInDrawnPoints] == null)
                {
                    _drawnObjects[indexInDrawnPoints] = Instantiate(tile, transform.position, quaternion.identity);
                    _drawnObjects[indexInDrawnPoints].name = $"TileDrawnBy{this.name}";
                }

                _drawnObjects[indexInDrawnPoints].gameObject.SetActive(true);
                GameObject current = _drawnObjects[indexInDrawnPoints];
                current.transform.localScale = mySplineBuilder.Tile.transform.localScale * sizeMultiplier;
                current.transform.position = points[i] + (points[i + offset] - points[i]) * 0.5f;
                current.transform.right = (points[i + offset] - points[i]).normalized;
                Vector3 currentLocalRotation = current.transform.localRotation.eulerAngles;
                current.transform.localRotation = Quaternion.Euler(currentLocalRotation.x,currentLocalRotation.y,currentLocalRotation.z+mySplineBuilder.offsetAngle);
                current.gameObject.transform.SetParent(gameObject.transform);
                i += offset;
                indexInDrawnPoints++;
            }
            usedPoints.Add(points[points.Length-1]);
        }
        private void OnDrawGizmos()
        {
            CheckForDeletedObjects();
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

        public void DeleteMyTiles()
        {
            while (_drawnObjects.Count > 0)
            {
                DestroyImmediate(_drawnObjects[0]);
                _drawnObjects.RemoveAt(0);
            }
        }

        public void DeleteMyUnseenTiles()
        {
            for (int i = 0; i < _drawnObjects.Count; i++)
            {
                if (!_drawnObjects[i].activeSelf)
                {
                    DestroyImmediate(_drawnObjects[i]);
                    i--;
                }
            }
        }
    }
}

