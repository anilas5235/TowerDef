using Unity.Mathematics;
using UnityEngine;

namespace Scrips.Towers
{
    public class WheelTower : TowerBase
    {
        [SerializeField] private GameObject[] barrelControllers;
        [SerializeField] private GameObject projectile;
        
        private GameObject[] _barrels = new GameObject[10];
        private int _numberOfBarrels = 6, _attackDamage = 1, _multiHit = 2;
        private float _attackDelay = 3;

        

        // Start is called before the first frame update
        protected override void Start()
        {
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
            _attackDelay -= 1f /2 * upgrade.x;
            _attackDamage +=  1 * (int)upgrade.y;
            _numberOfBarrels += 1 * (int)upgrade.z;

            VisualChange(); StatsKeeper.UpdateUI(); SetUpBarrelsForNewAngle();
            indicator.gameObject.transform.localScale = new Vector3(attackRadius*2, attackRadius*2, 1);
        }

        protected override void Attack()
        {
            if (Time.time >= timeForNextAttack && Physics2D.OverlapCircle(transform.position, attackRadius, enemyLayer))
            {
                for (int i = 0; i < _numberOfBarrels; i++)
                {
                    Projectile shoot = Instantiate(projectile, _barrels[i].transform.position, quaternion.identity).GetComponent<Projectile>();
                    shoot.pierce = _multiHit;
                    shoot.damage = _attackDamage;
                    shoot.targetDirection = (_barrels[i].transform.position-transform.position).normalized;
                    shoot.speed = 10;
                    shoot.projectileColor = ColorSequence(_attackDamage);
                    shoot.lifeTime = Time.time + (0.7f * attackRadius) / shoot.speed;
                    shoot.AppearanceUpdate();
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
