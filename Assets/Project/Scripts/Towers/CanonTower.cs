using Background.Audio;
using Background.Pooling;
using Scrips.Projectiles;
using Towers;
using UnityEngine;

namespace Scrips.Towers
{
    public class CanonTower : TowerBase
    {
        [SerializeField] protected Transform barrelTip;

        private StandardProjectilePool Pool;
        private float _attackDelay = 1; 
        private  int _attackDamage = 1, _multiHit = 1;
        
        protected override void Start()
        {
            Pool = StandardProjectilePool.Instance;
            attackRadius = 2.5f;
            Pool = FindObjectOfType<StandardProjectilePool>();
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
                AudioManager.Instance.PlayShootSound(11);
                Projectile shoot = Pool.GetObjectFromPool().GetComponent<Projectile>();
                shoot.ResetProjectileValues();
                shoot.gameObject.transform.position = barrelTip.position;
                shoot.pierce = _multiHit;
                shoot.damage = _attackDamage;
                shoot.targetDirection = targetDirection;
                shoot.speed = 5;
                shoot.AppearanceUpdate();
                shoot.pooled = false;
                timeForNextAttack = Time.time + _attackDelay;
            }
        }
    }
}