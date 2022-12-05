using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class TowerBasic : MonoBehaviour
{
    private bool placed = false, nowPlacable = true;
    private float attackRadius = 5f;

    private Camera cam;
    private GameObject Target,barrel;
    private SpriteRenderer Indicator;
    private float colloderRadius;

    [SerializeField] private LayerMask blockingLayer;

    [SerializeField] private LayerMask EnemysLayer;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        Indicator = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        Indicator.gameObject.transform.localScale = new Vector3(attackRadius, attackRadius, 1);
        colloderRadius = GetComponent<CircleCollider2D>().radius;
        barrel = gameObject.transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!placed)
        {
            nowPlacable = Physics2D.OverlapCircleAll(transform.position, colloderRadius, blockingLayer).Length <= 1 ;
            Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            transform.position = mousePosition;
            Indicator.color = nowPlacable ? new Color(1,1,1,0.2f) : new Color(1,0,0,0.2f);
            if (Input.GetMouseButtonDown(0)&& nowPlacable) { placed = true; Indicator.enabled = false; }
        }
        else
        {
            if (Target == null || Vector3.Distance(transform.position,Target.transform.position) > attackRadius )
            {
                Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(transform.position, attackRadius, EnemysLayer);
                Target = possibleTargets.Length > 0 ? possibleTargets[0].gameObject : null;
            }
            else
            {
                 barrel.transform.localRotation = Quaternion.AngleAxis(Vector2.Angle(transform.position-Target.transform.position,Vector2.up)+180f,Vector3.back);
            }
        }
    }

}
