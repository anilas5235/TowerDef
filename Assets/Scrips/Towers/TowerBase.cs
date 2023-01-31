using UnityEngine;

namespace Scrips
{
    public abstract class TowerBase : MonoBehaviour
    {
        public bool placed = false;
        private bool nowPlacable = true;
        public Vector3 upgradeLevel = Vector3.zero;
        [SerializeField] protected GameObject projectile;
        private Camera cam;
        protected GameObject Target,barrelPivotGameObject;
        protected SpriteRenderer mainBodySpriteRenderer;
        public SpriteRenderer Indicator;
        private float colloderRadius;
        [SerializeField] protected float _attackRadius = 3f;
        [SerializeField] protected Transform barrelTip;

        public bool selected = false;
        public TowerData TowerData;

        protected StatsKeeper statsKeeper;

        [SerializeField] protected LayerMask blockingLayer, towerLayer;
        [SerializeField] protected LayerMask EnemysLayer;
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
                if (Target == null || Vector3.Distance(transform.position,Target.transform.position) > _attackRadius )
                {
                    Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(transform.position, _attackRadius, EnemysLayer);
                    if (possibleTargets.Length < 1) { Target = null; return; }
                
                    float greatestdistance =0; int index = 0;
                    for (int i = 0; i < possibleTargets.Length; i++)
                    {
                        float currentDistance = possibleTargets[i].gameObject.GetComponent<Enemy>().distance;
                        if (currentDistance > greatestdistance) { greatestdistance = currentDistance; index = i; }
                    }
                    Target =  possibleTargets[index].gameObject;
                }
                else
                {
                   Attack();
                }
            }
        }
        public abstract void UpgradeTower(Vector3 upgrade);
        protected abstract void Attack();
    }
}
