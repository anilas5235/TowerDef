using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathKeeper : MonoBehaviour
{
    public static PathKeeper Instance;
    public GameObject[] PathPoints;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(this); }
    
        PathPoints = GameObject.FindGameObjectsWithTag("PathPoint");
    }
}
