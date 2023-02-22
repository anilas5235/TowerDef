using Unity.Mathematics;
using UnityEngine;

namespace Scrips.Towers
{
    public class SniperTower : TowerBase
    {
        private int _attackDamage = 3, _multiHit = 2, _attackDelay = 3;
        
        protected override void Start()
        {
            attackRadius = 20f;
            base.Start();
        }
        public override void UpgradeTower(Vector3 upgrade)
        {
            upgradeLevel += upgrade;
            _multiHit += (int) upgrade.x;
            _attackDamage += (int) upgrade.y*2;
            _attackDelay -= (int) upgrade.z/2;

            VisualChange(); StatsKeeper.UpdateUI();
        }

        protected override void Attack() 
        {
            Vector3 targetDirection = Target.transform.position - transform.position;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg % 360 - 90;
            BarrelPivotGameObject.transform.localRotation = Quaternion.Euler(0,0,angle);
            if (Time.time >= timeForNextAttack)
            {
                int allowedHits = _multiHit;
                RaycastHit2D[] possibleHitEnemies = Physics2D.RaycastAll(transform.position, targetDirection, 100f, enemyLayer);
                Debug.DrawRay(transform.position, targetDirection.normalized*100, Color.red,1f);

                foreach (RaycastHit2D hitEnemy in possibleHitEnemies)
                {
                    if(allowedHits <1){break;}
                    hitEnemy.collider.gameObject.GetComponent<Enemy>().TakeDamage(_attackDamage);
                    allowedHits--;
                }
                timeForNextAttack = Time.time + _attackDelay;
            }
        }

        protected override void VisualChange()
        {
            MainBodySpriteRenderer.color = ColorSequence(_attackDamage);
        }
    }
}
