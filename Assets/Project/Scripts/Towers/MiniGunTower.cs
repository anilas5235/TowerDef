using Background.Audio;
using Background.Pooling;
using Scrips.Projectiles;
using Scrips.Towers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Towers
{
    public class MiniGunTower : TowerBase
    {
        [SerializeField] protected Transform[] barrelTips;

        private StandardProjectilePool Pool;
        private int _attackDamage = 1, _multiHit = 1, _attackDelay = 4; //_timeForNextAttack = Time.time + 1/_attackDelay;

        protected override void Start()
        {
            Pool = StandardProjectilePool.Instance;
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
                AudioManager.Instance.PlayShootSound(1,7);
                Projectile shoot = Pool.GetObjectFromPool().GetComponent<Projectile>();
                shoot.ResetProjectileValues();
                shoot.gameObject.transform.position = barrelTips[Random.Range(0,2)].position;
                shoot.pierce = _multiHit;
                shoot.damage = _attackDamage;
                shoot.targetDirection = targetDirection;
                shoot.speed = 8;
                shoot.AppearanceUpdate();
                shoot.pooled = false;
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
