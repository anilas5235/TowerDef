using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathKeeper : MonoBehaviour
{
    public GameObject[] PathPoints;

    private void Awake()
    {
        PathPoints = GameObject.FindGameObjectsWithTag("PathPoint");
    }
}
