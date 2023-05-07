using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Background.SplinePath
{
    public abstract class BaseSplineBuilder : MonoBehaviour
    {
        [SerializeField] protected List<Transform> splinePoints = new List<Transform>();
        protected List<DrawCurve> DrawCurvesList = new List<DrawCurve>();
        [HideInInspector] public GameObject Point,DrawCurvePrefab;
        public GameObject Tile;
        [Range(0.01f, 1f)] public float RESOLUTION = 0.2f;
        public Color LineColor = Color.white;
        public float LineThickness = 0.5f;

        public virtual void AssembleSpline()
        {
            DeleteOldSpline();
            for (int i = 0; i < splinePoints.Count - 1; i++) SetUpSplineSegment(i);
        }
        public abstract void SetUpSplineSegment(int indexOfTheFirstPoint);

        protected void DeleteOldSpline()
        {
            foreach (DrawCurve drawCurve in DrawCurvesList) drawCurve.DeleteOldDraw();
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
        }

        public abstract void TriggerPointMoved(int index);
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
        public virtual Vector3 GetPoint(float t)
        {
            return new Matrix4x4(Points[0],Points[1],Points[2],Points[3]) * (MainMatrix * new Vector4(1, t, math.pow(t, 2), math.pow(t, 3)));
        }
    }
}
