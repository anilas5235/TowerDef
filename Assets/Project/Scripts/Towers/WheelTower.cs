using Background.Audio;
using Background.Pooling;
using Scrips.Projectiles;
using Scrips.Towers;
using UnityEngine;

namespace Towers
{
    public class WheelTower : TowerBase
    {
        [SerializeField] private GameObject[] barrelControllers;

        private GameObject[] _barrels = new GameObject[10];
        private StandardProjectilePool Pool;
        private int _numberOfBarrels = 6, _attackDamage = 1, _multiHit = 2;
        private float _attackDelay = 2;

        protected override void Start()
        {
            Pool = StandardProjectilePool.Instance;
            attackRadius = 1.5f;
            SetUpBarrelsForNewAngle();
            for (int i = 0; i < barrelControllers.Length; i++)
            {
                _barrels[i] = barrelControllers[i].transform.GetChild(0).gameObject;
            }
            base.Start();
        }

        public override void UpgradeTower(Vector3 upgrade)
        {
            upgradeLevel += upgrade;
            _attackDelay -= .3f * upgrade.x;
            _attackDamage +=  1 * (int)upgrade.y;
            _numberOfBarrels += 1 * (int)upgrade.z;

            VisualChange(); SetUpBarrelsForNewAngle();
            indicator.gameObject.transform.localScale = new Vector3(attackRadius*2, attackRadius*2, 1);
        }

        protected override void Attack()
        {
            if (Time.time >= timeForNextAttack && Physics2D.OverlapCircle(transform.position, attackRadius, enemyLayer))
            {
                for (int i = 0; i < _numberOfBarrels; i++)
                {
                    AudioManager.Instance.PlayShootSound();
                    Projectile shoot = Pool.GetObjectFromPool().GetComponent<Projectile>();
                    shoot.ResetProjectileValues();
                    shoot.gameObject.transform.position = _barrels[i].transform.position;
                    shoot.pierce = _multiHit;
                    shoot.damage = _attackDamage;
                    shoot.targetDirection = (_barrels[i].transform.position-transform.position).normalized;
                    shoot.speed = 5;
                    shoot.lifeTime = Time.time + (0.7f * attackRadius) / shoot.speed;
                    shoot.AppearanceUpdate();
                    shoot.pooled = false;
                }
                timeForNextAttack = Time.time + _attackDelay;
            }
        }

        protected override void VisualChange()
        {
            MainBodySpriteRenderer.color = ColorSequence(_attackDamage);
        }

        private void SetUpBarrelsForNewAngle()
        {
            for (int i = 0; i < barrelControllers.Length; i++)
            {
                if (_numberOfBarrels <= i) { barrelControllers[i].SetActive(false);continue; }
                barrelControllers[i].transform.localRotation = Quaternion.Euler(0,0,360/_numberOfBarrels*i);
                barrelControllers[i].SetActive(true);
            }
        }
    }
}
