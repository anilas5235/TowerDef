using Unity.Mathematics;
using UnityEngine;

namespace Scrips
{
    public abstract class TowerBase : MonoBehaviour
    {
        public bool placed = false;
        private bool nowPlacable = true;
        public Vector3 upgradeLevel = Vector3.zero;
        [SerializeField] private GameObject Projectile;
        private Camera cam;
        private GameObject Target,barrelPivotGameObject;
        protected SpriteRenderer mainBodySpriteRenderer;
        public SpriteRenderer Indicator;
        private float colloderRadius;
        [SerializeField] private Transform barrelTip;

        public bool selected = false;
        public TowerData TowerData;

        protected StatsKeeper statsKeeper;

        [SerializeField] private LayerMask blockingLayer, towerLayer;

        [SerializeField] private LayerMask EnemysLayer;
        // Start is called before the first frame update
        protected virtual void Start()
        {
            cam = Camera.main;
            mainBodySpriteRenderer = GetComponent<SpriteRenderer>();
            colloderRadius = GetComponent<CircleCollider2D>().radius;
            barrelPivotGameObject = gameObject.transform.GetChild(1).gameObject;
            statsKeeper = StatsKeeper.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            if (!placed)
            {
                nowPlacable = Physics2D.OverlapCircleAll(transform.position, colloderRadius, blockingLayer + towerLayer).Length <= 1 && statsKeeper.Money >= TowerData.placingCosts;
                Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                transform.position = mousePosition;
                Indicator.color = nowPlacable ? new Color(1,1,1,0.2f) : new Color(1,0,0,0.2f);
                if (Input.GetMouseButtonDown(0)&& nowPlacable) { placed = true; Indicator.enabled = false; statsKeeper.Money -= TowerData.placingCosts; statsKeeper.UpdateUI(); }
                else if(Input.GetMouseButtonDown(1)) { Destroy(gameObject); }
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
                    barrelPivotGameObject.transform.localRotation = Quaternion.Euler(0,0,angle);
                    if (Time.time >= timeForNextAttack)
                    {
                        Projectile shoot = Instantiate(Projectile, barrelTip.position, quaternion.identity).GetComponent<Projectile>();
                        shoot.pierce = multiHit;
                        shoot.damage = attackDamage;
                        shoot.targetDirection = offset;
                        shoot.speed = 2 + attackDamage / 2;
                        shoot.projectileColor = SetColor();
                        timeForNextAttack = Time.time + attackDelay;
                    }
                    Debug.DrawLine(transform.position, Target.transform.position,Color.red,0.001f);
                }
            }
        }
    
        
    }
}
