using Scrips.Background;
using UnityEngine;

namespace Scrips.Towers
{
    public abstract class TowerBase : MonoBehaviour
    {
        public Vector3 upgradeLevel = Vector3.zero;
        public bool placed = false;
        public bool selected = false;
        public SpriteRenderer indicator; 
        public TowerData towerData;
        
        private Camera _camera;
        private float _colliderRadius;
        private bool _nowPlacable = true;
        
        protected GameObject Target,BarrelPivotGameObject;
        protected SpriteRenderer MainBodySpriteRenderer;
        protected StatsKeeper StatsKeeper;
        protected Shop Shop;
        protected float attackRadius,timeForNextAttack;

        [SerializeField] protected Transform barrelTip;
        [SerializeField] protected LayerMask blockingLayer, towerLayer;
        [SerializeField] protected LayerMask enemyLayer;

        protected virtual void Start()
        {
            _camera = Camera.main;
            MainBodySpriteRenderer = GetComponent<SpriteRenderer>();
            _colliderRadius = GetComponent<CircleCollider2D>().radius;
            BarrelPivotGameObject = gameObject.transform.GetChild(1).gameObject;
            StatsKeeper = StatsKeeper.Instance;
            Shop = Background.Shop.Instance;
            timeForNextAttack = Time.time;
            indicator.gameObject.transform.localScale = new Vector3(attackRadius*2, attackRadius*2, 1);
            VisualChange();
        }

        void Update()
        {
            if (!placed)
            {
                _nowPlacable = Physics2D.OverlapCircleAll(transform.position, _colliderRadius, blockingLayer + towerLayer).Length <= 1
                               && StatsKeeper.Money >= towerData.placingCosts;
               
                Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                transform.position = mousePosition;
                
                indicator.color = _nowPlacable ? new Color(1,1,1,0.2f) : new Color(1,0,0,0.2f);
                if (Input.GetMouseButtonDown(0) && _nowPlacable)
                {
                    placed = true;
                    Shop.TowerHandled();
                    indicator.enabled = false;
                    StatsKeeper.Money -= towerData.placingCosts;
                    StatsKeeper.UpdateUI();
                }
                else if(Input.GetMouseButtonDown(1)) { Destroy(gameObject);Shop.TowerHandled(); }
            }
            else
            {
                if (Target == null || Vector3.Distance(transform.position,Target.transform.position) > attackRadius )
                {
                    Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(transform.position, attackRadius, enemyLayer);
                    if (possibleTargets.Length < 1) { Target = null; return; }
                
                    float greatestdistance =0; int index = 0;
                    for (int i = 0; i < possibleTargets.Length; i++)
                    {
                        float currentDistance = possibleTargets[i].gameObject.GetComponent<Enemy>().distance;
                        if (currentDistance > greatestdistance)
                        {
                            greatestdistance = currentDistance;
                            index = i;
                        }
                    }
                    Target =  possibleTargets[index].gameObject;
                }
                else { Attack(); }
            }
        }
        public abstract void UpgradeTower(Vector3 upgrade);
        protected abstract void Attack();

        protected abstract void VisualChange();

        protected Color ColorSequence(int index)
        {
            Color currentColor = Color.red;
            switch (index)
            {
                case 1: currentColor = Color.red;  break;
                case 2: currentColor = Color.blue; ; break;
                case 3: currentColor = Color.green;  break;
                case 4: currentColor = Color.yellow;  break;
                case 5: currentColor = Color.cyan;  break;
                case 6: currentColor = Color.grey;  break;
                case 7: currentColor = Color.black;  break;
                case 8: currentColor = Color.white;  break;
            
                default: print("Color for "+ index + " attackdamage is not defined"); break;
            }
            return currentColor;
        }
    }
}
