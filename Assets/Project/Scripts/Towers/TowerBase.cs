using System.Collections.Generic;
using Background.Keeper;
using Background.WaveManaging;
using UIScripts.ShopUi;
using UnityEngine;

namespace Towers
{
    public abstract class TowerBase : MonoBehaviour
    {
        [SerializeField] private float blockRadius = 0.5f;
        public Vector3 upgradeLevel = Vector3.zero;
        public bool placed;
        public SpriteRenderer indicator; 
        public TowerData towerData;
        public bool needsTargetAtAll = true;
        private int towerInvestmentsTillNow = 0;

        public int TowerInvestmentsTillNow
        { get => towerInvestmentsTillNow;
            set
            {
                if (value >0) towerInvestmentsTillNow += value;
            }
        }
            
        private bool _nowPlaceable = true;
        private Collider2D ownCollider;
        private ContactFilter2D _placeableFilter;
        
        protected Camera _camera;
        protected GameObject Target,BarrelPivotGameObject;
        protected SpriteRenderer MainBodySpriteRenderer;
        protected StatsKeeper StatsKeeper;
        protected Shop Shop;
        protected float attackRadius,timeForNextAttack;

        [SerializeField] protected LayerMask blockingLayer, towerLayer;
        [SerializeField] protected LayerMask enemyLayer;

        protected virtual void Start()
        {
            _placeableFilter.NoFilter(); _placeableFilter.SetLayerMask(blockingLayer+towerLayer);
            _placeableFilter.SetDepth(-50,50);
            _camera = Camera.main;
            MainBodySpriteRenderer = GetComponent<SpriteRenderer>();
            ownCollider = GetComponent<Collider2D>();
            BarrelPivotGameObject = gameObject.transform.GetChild(1).gameObject;
            StatsKeeper = StatsKeeper.Instance;
            Shop = Shop.Instance;
            timeForNextAttack = Time.time;
            indicator.gameObject.transform.localScale = new Vector3(attackRadius*2, attackRadius*2, 1);
            VisualChange();
        }

        protected virtual void Update()
        {
            if (!placed)
            {
                List<Collider2D> cols = new List<Collider2D>();
                ownCollider.OverlapCollider(_placeableFilter, cols);               

                Collider[] result = new Collider[1];
                Physics.OverlapSphereNonAlloc(transform.position, blockRadius, result);
                _nowPlaceable = result[0] == null && cols.Count <1 && StatsKeeper.Money >= towerData.placingCosts;
               
                Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                transform.position = mousePosition;
                
                indicator.color = _nowPlaceable ? new Color(1,1,1,0.2f) : new Color(1,0,0,0.2f);
                if (Input.GetMouseButtonDown(0) && _nowPlaceable)
                {
                    placed = true;
                    Shop.TowerHandled();
                    indicator.enabled = false;
                    int cost = (int)(towerData.placingCosts * Shop.Instance.priceMultiplier);
                    StatsKeeper.Money -= cost;
                    TowerInvestmentsTillNow += cost;
                }
                else if(Input.GetMouseButtonDown(1)) { Destroy(gameObject);Shop.TowerHandled(); }
            }
            else
            {
                if (Target != null) { if(!Target.activeInHierarchy) Target = null; }

                if (SpawnManager.Instance.waveIsRunning == false) return;
                if(!needsTargetAtAll){ Attack(); return;}
                if (Target == null || Vector3.Distance(transform.position,Target.transform.position) > attackRadius )
                {
                    Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(transform.position, attackRadius, enemyLayer);
                    if (possibleTargets.Length < 1) { Target = null; return; }
                
                    float greatestdistance =0; int index = 0;
                    for (int i = 0; i < possibleTargets.Length; i++)
                    {
                        if(!possibleTargets[i].gameObject.activeInHierarchy){continue;}
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

        protected static Color ColorSequence(int index)
        {
            return ColorKeeper.StandardColors(index-1);
        }
    }
}
