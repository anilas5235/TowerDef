using System.Collections.Generic;
using Scrips.Background;
using Scrips.Background.WaveManaging;
using UnityEngine;

namespace Scrips.Towers
{
    public abstract class TowerBase : MonoBehaviour
    {
        public Vector3 upgradeLevel = Vector3.zero;
        public bool placed;
        public SpriteRenderer indicator; 
        public TowerData towerData;
        public bool needsTargetAtAll = true;
        
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
                _nowPlaceable = cols.Count <1 && StatsKeeper.Money >= towerData.placingCosts;
               
                Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                transform.position = mousePosition;
                
                indicator.color = _nowPlaceable ? new Color(1,1,1,0.2f) : new Color(1,0,0,0.2f);
                if (Input.GetMouseButtonDown(0) && _nowPlaceable)
                {
                    placed = true;
                    Shop.TowerHandled();
                    indicator.enabled = false;
                    StatsKeeper.Money -= towerData.placingCosts;
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