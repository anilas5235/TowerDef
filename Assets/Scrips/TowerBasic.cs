using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class TowerBasic : MonoBehaviour
{
    private bool placed = false, nowPlacable = true;
    private float attackRadius = 3f; 
    private  int attackDamage = 1, multiHit = 1;
    private Vector3 UpgradeLevel = Vector3.zero;

    private Camera cam;
    private GameObject Target,barrel;
    private SpriteRenderer Indicator,MainBody;
    private float colloderRadius;

    [SerializeField] private Tower TowerData;

    private StatsKeeper StatsKeeper;

    [SerializeField] private LayerMask blockingLayer;

    [SerializeField] private LayerMask EnemysLayer;
    // Start is called before the first frame update
    void Start()
    {
        UpgradeLevel = new Vector3(attackRadius, attackDamage, multiHit);
        cam = Camera.main;
        Indicator = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        MainBody = GetComponent<SpriteRenderer>();
        Indicator.gameObject.transform.localScale = new Vector3(attackRadius*2, attackRadius*2, 1);
        colloderRadius = GetComponent<CircleCollider2D>().radius;
        barrel = gameObject.transform.GetChild(1).gameObject;
        StatsKeeper = StatsKeeper.Instance;
        
        SetColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (!placed)
        {
            nowPlacable = Physics2D.OverlapCircleAll(transform.position, colloderRadius, blockingLayer).Length <= 1 && StatsKeeper.Money >= TowerData.placingCosts;
            Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            transform.position = mousePosition;
            Indicator.color = nowPlacable ? new Color(1,1,1,0.2f) : new Color(1,0,0,0.2f);
            if (Input.GetMouseButtonDown(0)&& nowPlacable) { placed = true; Indicator.enabled = false; StatsKeeper.Money -= TowerData.placingCosts; StatsKeeper.UpdateUI(); }
        }
        else
        {
            if (Target == null || Vector3.Distance(transform.position,Target.transform.position) > attackRadius )
            {
                Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(transform.position, attackRadius, EnemysLayer);
                if (possibleTargets.Length < 1) { Target = null; return; }
                
                float highestdistance =0; int index = 0;
                for (int i = 0; i < possibleTargets.Length; i++)
                {
                    float currentDistance = possibleTargets[i].gameObject.GetComponent<Enemy>().distance;
                    if (currentDistance > highestdistance) { highestdistance = currentDistance; index = i; }
                }
                Target =  possibleTargets[index].gameObject;
            }
            else
            {
                Vector3 offset = Target.transform.position - transform.position;
                float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg % 360 - 90;
                barrel.transform.localRotation = Quaternion.Euler(0,0,angle);
                 
                print("currentTarget "+Target);
                Debug.DrawLine(transform.position, Target.transform.position,Color.red,0.001f);
            }
        }
    }
    
    private void SetColor()
    {
        Color currentColor = Color.red;
        switch (attackDamage)
        {
            case 1: currentColor = Color.red;  break;
            case 2: currentColor = Color.blue; ; break;
            case 3: currentColor = Color.green;  break;
            case 4: currentColor = Color.yellow;  break;
            case 5: currentColor = Color.cyan;  break;
            case 6: currentColor = Color.grey;  break;
            case 7: currentColor = Color.black;  break;
            case 8: currentColor = Color.white;  break;
            
            default: print("Color for "+ attackDamage+ " attackdamage is not defined"); break;
        }

        MainBody.color = currentColor;
    }

    public void UpgradeTower(Vector3 upgrade, int cost)
    {
        StatsKeeper.Money -= cost; StatsKeeper.UpdateUI();
        UpgradeLevel += upgrade;

        attackRadius = UpgradeLevel.x;
        attackDamage = (int) UpgradeLevel.y;
        multiHit = (int) UpgradeLevel.z;
        
        SetColor();
    }

}
