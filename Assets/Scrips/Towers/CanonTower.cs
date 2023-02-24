using Unity.Mathematics;
using UnityEngine;

namespace Scrips.Towers
{
    public class CanonTower : TowerBase
    {
        [SerializeField] protected GameObject projectile;
        [SerializeField] protected Transform barrelTip;
        
        private float _attackDelay = 1; 
        private  int _attackDamage = 1, _multiHit = 2;

        protected override void Start()
        {
            attackRadius = 2.5f;
            base.Start();
        }

        protected override void VisualChange()
        {
            MainBodySpriteRenderer.color = ColorSequence(_attackDamage);

            barrelTip.localScale = new Vector3(0.1f + (_multiHit  / 10f), barrelTip.localScale.y, barrelTip.localScale.z);
        }

        public override void UpgradeTower(Vector3 upgrade)
        {
            upgradeLevel += upgrade;
            attackRadius += 1f /3 * upgrade.x;
            _attackDamage +=  1 * (int)upgrade.y;
            _multiHit += 1 * (int)upgrade.z;

            VisualChange(); StatsKeeper.UpdateUI();
            indicator.gameObject.transform.localScale = new Vector3(attackRadius*2, attackRadius*2, 1);
        }

        protected override void Attack()
        {
            Vector3 targetDirection = Target.transform.position - transform.position;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg % 360 - 90;
            BarrelPivotGameObject.transform.localRotation = Quaternion.Euler(0,0,angle);
            if (Time.time >= timeForNextAttack)
            {
                Projectile shoot = Instantiate(projectile, barrelTip.position, quaternion.identity).GetComponent<Projectile>();
                shoot.pierce = _multiHit;
                shoot.damage = _attackDamage;
                shoot.targetDirection = targetDirection;
                shoot.speed = 5;
                shoot.projectileColor = MainBodySpriteRenderer.color;
                shoot.AppearanceUpdate();
                timeForNextAttack = Time.time + _attackDelay;
            }
        }
    }
}