using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
   
    private PathKeeper PathKeeper;
    private int nextPointInArry = 0;
    public int hp = 1;

    private float speed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        PathKeeper = GameObject.FindObjectOfType<PathKeeper>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate((PathKeeper.PathPoints[nextPointInArry].transform.position - transform.position).normalized * Time.deltaTime * speed);
        if (Vector3.Distance( transform.position, PathKeeper.PathPoints[nextPointInArry].transform.position) < 0.1f)
        {
            nextPointInArry++;
            print(""+PathKeeper.PathPoints[nextPointInArry].transform.position);
        }
    }
}
