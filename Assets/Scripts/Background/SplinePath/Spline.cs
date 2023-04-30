using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Background.SplinePath
{
    public class Spline : MonoBehaviour
    {
        [HideInInspector] public SplineType usedSplineType;
        [SerializeField] private GameObject Tile;
        [SerializeField, Range(0.01f, 1f)] private float RESOLUTION = 0.2f;
        public List<Transform> splinePoints = new List<Transform>();
        
        private SplineType current;
        private GameObject Point, LineRendererPrefab, DrawCurvePrefab;
        private BSplineMaster RefBSplineMaster;
        private BaseSplineBuilder currentSpline;
        public enum SplineType
        {
            BezieSpline = 1,
            BSpline = 2,
        }

        public void SplineTypeChange()
        {
            if (usedSplineType == current) return;
            switch (current)
            {
                case SplineType.BezieSpline:
                    break;
                case SplineType.BSpline:
                    if (RefBSplineMaster) RefBSplineMaster.enabled = false;
                    break;
            }
            current = usedSplineType;
            switch (usedSplineType)
            {
                case SplineType.BezieSpline:
                    break;
                case SplineType.BSpline:

                    if (!RefBSplineMaster)
                    {
                        if (gameObject.TryGetComponent(typeof(BSplineMaster), out var component))
                        {
                            RefBSplineMaster = (BSplineMaster)component;
                        }
                        else
                        {
                            RefBSplineMaster = gameObject.AddComponent<BSplineMaster>();
                            RefBSplineMaster.DrawCurvePrefab = DrawCurvePrefab;
                            RefBSplineMaster.LineRendererPrefab = LineRendererPrefab;
                        }
                    }
                    RefBSplineMaster.enabled = true;
                    break;
                default:
                    Debug.Log("SplineType is not set a valid Value");
                    break;
            }
        }
        public void AssembleSpline()
        {
            switch (usedSplineType)
            {
                case SplineType.BezieSpline:
                    break;
                case SplineType.BSpline:
                    RefBSplineMaster.splinePoints = splinePoints;
                    RefBSplineMaster.AssembleSpline();
                    break;
            } 
        }

        public void AddPoint()
        {
            Transform newPoint = Instantiate(Point, transform.position, quaternion.identity).transform;
            switch (usedSplineType)
            {
                case SplineType.BezieSpline:
                    break;
                case SplineType.BSpline: RefBSplineMaster.AddPointToSpline(newPoint);
                    break;
            }
        }

        public void LoadPrefaps()
        {
            GameObject[] mySources = Resources.LoadAll<GameObject>("SplineStuff");
            DrawCurvePrefab = mySources[0];
            LineRendererPrefab = mySources[1];
            Point = mySources[2];
        }
        
    }
}