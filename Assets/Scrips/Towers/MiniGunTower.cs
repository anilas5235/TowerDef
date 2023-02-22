using System;
using Unity.Mathematics;
using UnityEngine;

namespace Scrips.Towers
{
    public class MiniGunTower : TowerBase
    {
        [SerializeField] protected GameObject projectile;

        private int _attackDamage = 1, _multiHit = 1, _attackDelay = 2; //_timeForNextAttack = Time.time + 1/_attackDelay;

        protected override void Start()
        {
            attackRadius = 3.5f;
            base.Start();
        }

        public override void UpgradeTower(Vector3 upgrade)
        {
            upgradeLevel += upgrade;
            attackRadius += upgrade.x / 3;
            _attackDamage += (int) upgrade.y;
            _attackDelay += (int) upgrade.z;

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
                shoot.speed = 10;
                shoot.projectileColor = ColorSequence(_attackDamage);
                shoot.AppearanceUpdate();
                timeForNextAttack = Time.time + 1f/_attackDelay;
            }
        }

        protected override void VisualChange()
        {
            MainBodySpriteRenderer.color = ColorSequence(_attackDamage);
            // still need to integrate the ration of the barrels -> animation ...
        }
    }
}
