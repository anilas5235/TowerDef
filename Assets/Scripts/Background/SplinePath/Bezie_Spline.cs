using System;

using Background.SplinePath;
using UnityEditor;
using UnityEngine;

public class Bezie_Spline : BaseSplineBuilder
{
    public bool mirrorSplineArms = true;
    public override void SetUpSplineSegment(int indexOfTheFirstPoint)
    {
        if (indexOfTheFirstPoint < 0|| indexOfTheFirstPoint > splinePoints.Count-1) return;
        MySplineType = SplineType.Bezie_Spline;
        foreach (Transform splinePoint in splinePoints)
        {
            if (splinePoint == null)
            {
                this.AssembleSpline();
                return;
            }
        }

        DrawCurve current;

        int indexOfSegment = (int) Math.Floor(indexOfTheFirstPoint / 3f);
        for (int i = 0; i < DrawCurvesList.Count; i++)
        {
            if (DrawCurvesList[i] == null)
            {
                DrawCurvesList.RemoveAt(i);
                i--;
            }
        }
        if (indexOfSegment > DrawCurvesList.Count - 1 && indexOfSegment*3 < splinePoints.Count-1)
        {
            DrawCurvesList.Add(((GameObject)PrefabUtility.InstantiatePrefab(DrawCurvePrefab))
                .GetComponent<DrawCurve>());
            DrawCurvesList[DrawCurvesList.Count - 1].gameObject.name = $"Segment{DrawCurvesList.Count - 1}";
            DrawCurvesList[DrawCurvesList.Count - 1].gameObject.transform.SetParent(transform);
        }

        if (indexOfSegment > DrawCurvesList.Count - 1)
        {
            current = DrawCurvesList[DrawCurvesList.Count - 1];
        }
        else
        {
            current = DrawCurvesList[indexOfSegment];
        }
        indexOfSegment *= 3;
        if (indexOfSegment+3 > splinePoints.Count-1) return;
        current.pointsForTheCurve = new Transform[]
        {
            splinePoints[indexOfSegment], splinePoints[indexOfSegment + 1], splinePoints[indexOfSegment + 2],
            splinePoints[indexOfSegment + 3]
        };

        current.mySplineBuilder = this;
        current.Draw();
    }

    public override void AssembleSpline()
    {
        MySplineType = SplineType.Bezie_Spline;
       
        if (splinePoints.Count < 4)return;
        base.AssembleSpline();
        int index = 1;
        do
        {
            SetUpSplineSegment(index);
            index += 3;
        } while (index < splinePoints.Count);
    }


    protected override void UpdatePointsAdded()
    {
        
    }

    public override void TriggerPointMoved(int index)
    {
        float floatIndex = index / 3f;
        SetUpSplineSegment(index);
        if (floatIndex%1 < 0.01f)
        {
            SetUpSplineSegment(index-3);
        }

        if (mirrorSplineArms && index > 1 && index < splinePoints.Count-2 && floatIndex%1 !=0)
        {
            if (floatIndex%1 < 0.5f)
            {
                splinePoints[index-2].transform.position = splinePoints[index - 1].transform.position +
                                                           (splinePoints[index - 1].transform.position -
                                                            splinePoints[index].transform.position);
                SetUpSplineSegment(index-3);
            }
            else
            {
                splinePoints[index+2].transform.position = splinePoints[index + 1].transform.position +
                                                     (splinePoints[index + 1].transform.position -
                                                      splinePoints[index].transform.position);
                SetUpSplineSegment(index+3);
            }
        }
    }

    public override void InitializeSpline()
    {
        if (splinePoints.Count > 0) return;
        LoadPrefabs();
        Vector3 position = transform.position;
        AddPointToSpline(position + Vector3.left);
        AddPointToSpline(position + new Vector3(-1, 1, 0));
        AddPointToSpline(position + new Vector3(1, 1, 0));
        AddPointToSpline(position + Vector3.right);
    }

    public void AddSegment()
    {
        if (splinePoints.Count < 1) 
        {
            InitializeSpline();
            return;
        }

        Vector3 positionOfLastPoint = splinePoints[splinePoints.Count - 1].position;
        Vector3 directionForNewSegment = (positionOfLastPoint - splinePoints[splinePoints.Count - 2].position).normalized;
        for (int i = 1; i < 4; i++)
        {
            AddPointToSpline(positionOfLastPoint+directionForNewSegment*i);
        }
        for (int i = 0; i < splinePoints.Count - 1; i += 3)
        {
            splinePoints[i + 1].transform.SetParent(splinePoints[i].transform);
            splinePoints[i + 2].transform.SetParent(splinePoints[i + 3].transform);
        }
        SetUpSplineSegment(splinePoints.Count-1);
    }

    public void InitialMirrorArmPoints()
    {
        foreach (Transform splinePoint in splinePoints)
        {
            if (splinePoint == null){ return; }
        }
        for (int i = 2; i < splinePoints.Count-3; i+=3)
        {
            splinePoints[i+2].transform.position = splinePoints[i + 1].transform.position +
                                                       (splinePoints[i + 1].transform.position -
                                                        splinePoints[i].transform.position);
        }
        AssembleSpline();
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.grey;
        for (int i = 0; i < splinePoints.Count-1; i+=3)
        {
            Handles.DrawLine(splinePoints[i].position,splinePoints[i+1].position,5f);
            Handles.DrawLine(splinePoints[i+2].position,splinePoints[i+3].position,5f);
        }
    }
}

