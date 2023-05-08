using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Background.SplinePath
{
    public abstract class BaseSplineBuilder : MonoBehaviour
    {
        [HideInInspector] public GameObject Point,DrawCurvePrefab;
        public GameObject Tile;
        [Range(0.01f, 1f)] public float RESOLUTION = 0.2f;
        public Color LineColor = Color.white;
        public float LineThickness = 0.5f;
        [SerializeField] protected List<Transform> splinePoints = new List<Transform>();
        [SerializeField] protected List<DrawCurve> DrawCurvesList = new List<DrawCurve>();

        private Color currentLineColor = default;
        private float currentLineThickness = default, currentRESOLUTION = default;

        public virtual void AssembleSpline()
        {
            if (splinePoints.Count < 1 || DrawCurvesList.Count <1) CheckForExistingComponents();
            for (int i = 0; i < splinePoints.Count; i++)
            {
                if (splinePoints[i] == null)
                {
                    splinePoints.RemoveAt(i);
                    i--;
                }
                else
                {
                    splinePoints[i].gameObject.GetComponent<PointBehaviour>().index = i;
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(splinePoints[i].gameObject.GetComponent<PointBehaviour>());
#endif
                }
            }
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
#endif
            DeleteOldSpline();
            for (int i = 0; i < splinePoints.Count - 1; i++) SetUpSplineSegment(i);
        }
        public abstract void SetUpSplineSegment(int indexOfTheFirstPoint);

        protected void DeleteOldSpline()
        {
            for (int i = 0; i < DrawCurvesList.Count; i++)
            {
                if (splinePoints.Count-3 < i)
                {
                    DestroyImmediate(DrawCurvesList[i].gameObject);
                    DrawCurvesList.RemoveAt(i);
                    i--;
                }
                else
                {
                    DrawCurvesList[i].DeleteOldDraw();
                }
            }
        }

        public void AddPointToSpline()
        {
            GameObject newPoint = (GameObject)PrefabUtility.InstantiatePrefab(Point);
            switch (splinePoints.Count)
            {
                case 0:
                    newPoint.transform.position = transform.position;
                    break;
                case 1:
                    newPoint.transform.position = splinePoints[splinePoints.Count - 1].position;
                    break;
                default:
                    newPoint.transform.position = splinePoints[splinePoints.Count - 1].position
                                                  + (splinePoints[splinePoints.Count - 1].position -
                                                     splinePoints[splinePoints.Count - 2].position).normalized;
                    break;
            }
            PointBehaviour current = newPoint.GetComponent<PointBehaviour>();
            current.Master = this;
            current.index = splinePoints.Count;
            splinePoints.Add(newPoint.transform);
            splinePoints[splinePoints.Count - 1].transform.SetParent(transform);
            UpdatePointsAdded();
        }

        public void AddPointToSpline(Vector3 position)
        {
            GameObject newPoint = (GameObject)PrefabUtility.InstantiatePrefab(Point);
            newPoint.transform.position = position;
            PointBehaviour current = newPoint.GetComponent<PointBehaviour>();
            current.Master = this;
            current.index = splinePoints.Count;
            splinePoints.Add(newPoint.transform);
            splinePoints[splinePoints.Count - 1].transform.SetParent(transform);
            UpdatePointsAdded();
        }
        protected abstract void UpdatePointsAdded();
        public void LoadPrefabs()
        {
            GameObject[] mySources = Resources.LoadAll<GameObject>("SplineStuff");
            DrawCurvePrefab = mySources[0];
            Point = mySources[1];
            CheckForExistingComponents();
        }

        public abstract void TriggerPointMoved(int index);

        public void CheckForExistingComponents()
        {
            splinePoints.Clear();
            foreach (var point in gameObject.GetComponentsInChildren<PointBehaviour>().ToList())
            {
                splinePoints.Add(point.gameObject.transform);
                point.Master = this;
            }

            DrawCurvesList = gameObject.GetComponentsInChildren<DrawCurve>().ToList();
        }
        
        protected virtual void OnDrawGizmosSelected()
        {
            bool needUpdate = false;
            if (currentLineColor != LineColor)
            {
                needUpdate = true;
                currentLineColor = LineColor;
            }

            if (math.abs( currentRESOLUTION - RESOLUTION) > 0.009f)
            {
                needUpdate = true;
                currentRESOLUTION = RESOLUTION;
            }

            if (math.abs( currentLineThickness- LineThickness) > 0.009f)
            {
                needUpdate = true;
                currentLineThickness = LineThickness;
            }

            if (needUpdate) AssembleSpline();
        }
    }
    
    public class BezierHermiteSpline : CurveBase
    {
        private static readonly Matrix4x4 SpecificMainMatrix = new Matrix4x4(new Vector4(1,0,0,0),new Vector4(0,1,0,0), new Vector4(-3,-2,3,-1),new Vector4(2,1,-2,1));
        
        public BezierHermiteSpline(Transform point1,Vector3 velocity1,Transform point2,Vector3 velocity2)
        {
            Points = new Vector3[4];
            Points[0] = point1.position;
            Points[1] = velocity1;
            Points[2] = point2.position;
            Points[3] = velocity2;
            MainMatrix = SpecificMainMatrix;
        }
    }

    public abstract class CurveBase
    {
        protected Matrix4x4 MainMatrix;
        protected Vector3[] Points;
        public Vector3 GetPoint(float t)
        {
            return new Matrix4x4(Points[0],Points[1],Points[2],Points[3]) * (MainMatrix * new Vector4(1, t, math.pow(t, 2), math.pow(t, 3)));
        }
    }
}
