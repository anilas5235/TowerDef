using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Background.SplinePath
{
    public abstract class BaseSplineBuilder : MonoBehaviour
    {
        [SerializeField] protected List<Transform> splinePoints = new List<Transform>();
        protected List<DrawCurve> DrawCurvesList = new List<DrawCurve>();
        [HideInInspector] public GameObject Point, LineRendererPrefab,DrawCurvePrefab;
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

        public abstract void AddPointToSpline();
        public void LoadPrefabs()
        {
            GameObject[] mySources = Resources.LoadAll<GameObject>("SplineStuff");
            DrawCurvePrefab = mySources[0];
            LineRendererPrefab = mySources[1];
            Point = mySources[2];
        }
    }
    
    public class BezierHermiteSpline : CurveBase
    {
        private static readonly Matrix4x4 MainMatrix = new Matrix4x4(new Vector4(1,0,0,0),new Vector4(0,1,0,0), new Vector4(-3,-2,3,-1),new Vector4(2,1,-2,1));
        
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
