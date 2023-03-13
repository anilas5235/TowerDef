using Scrips.Background;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scrips.Towers
{
    public class MiniGunTower : TowerBase
    {
        [SerializeField] protected Transform[] barrelTips;

        private int _attackDamage = 1, _multiHit = 1, _attackDelay = 4; //_timeForNextAttack = Time.time + 1/_attackDelay;

        protected override void Start()
        {
            attackRadius = 3f;
            base.Start();
        }

        public override void UpgradeTower(Vector3 upgrade)
        {
            upgradeLevel += upgrade;
            attackRadius += 1f/4 * upgrade.x ;
            _attackDamage += 1 * (int) upgrade.y;
            _attackDelay += 1 * (int) upgrade.z;

            VisualChange();
            indicator.gameObject.transform.localScale = new Vector3(attackRadius*2, attackRadius*2, 1);
        }

        protected override void Attack()
        {
            Vector3 targetDirection = Target.transform.position - transform.position;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg % 360 - 90;
            BarrelPivotGameObject.transform.localRotation = Quaternion.Euler(0,0,angle);
            if (Time.time >= timeForNextAttack)
            {
                Projectile shoot = ProjectilePooling.Instance.GetStandardProjectileFromPool().GetComponent<Projectile>();
                shoot.gameObject.transform.position = barrelTips[Random.Range(0,2)].position;
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
