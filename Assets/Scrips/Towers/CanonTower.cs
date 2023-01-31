using Unity.Mathematics;
using UnityEngine;

namespace Scrips.Towers
{
    public class CanonTower : TowerBase
    {
        private float _attackDelay = 1, _timeForNextAttack; 
        private  int _attackDamage = 1, _multiHit = 2;

        protected override void Start()
        {
            base.Start();
            indicator.gameObject.transform.localScale = new Vector3(attackRadius*2, attackRadius*2, 1);
            _timeForNextAttack = Time.time;
            VisualChange();
        }
        
        private void VisualChange()
        {
            Color currentColor = Color.red;
            switch (_attackDamage)
            {
                case 1: currentColor = Color.red;  break;
                case 2: currentColor = Color.blue; ; break;
                case 3: currentColor = Color.green;  break;
                case 4: currentColor = Color.yellow;  break;
                case 5: currentColor = Color.cyan;  break;
                case 6: currentColor = Color.grey;  break;
                case 7: currentColor = Color.black;  break;
                case 8: currentColor = Color.white;  break;
            
                default: print("Color for "+ _attackDamage+ " attackdamage is not defined"); break;
            }
            MainBodySpriteRenderer.color = currentColor;

            barrelTip.localScale = new Vector3(0.1f + (_multiHit  / 10f), barrelTip.localScale.y, barrelTip.localScale.z);
        }

        public override void UpgradeTower(Vector3 upgrade)
        {
            upgradeLevel += upgrade;
            attackRadius += upgrade.x / 2;
            _attackDamage += (int) upgrade.y;
            _multiHit += (int) upgrade.z;

            VisualChange(); StatsKeeper.UpdateUI();
            indicator.gameObject.transform.localScale = new Vector3(attackRadius*2, attackRadius*2, 1);
        }

        protected override void Attack()
        {
            Vector3 targetDirection = Target.transform.position - transform.position;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg % 360 - 90;
            BarrelPivotGameObject.transform.localRotation = Quaternion.Euler(0,0,angle);
            if (Time.time >= _timeForNextAttack)
            {
                Projectile shoot = Instantiate(projectile, barrelTip.position, quaternion.identity).GetComponent<Projectile>();
                shoot.pierce = _multiHit;
                shoot.damage = _attackDamage;
                shoot.targetDirection = targetDirection;
                shoot.speed = 5;
                shoot.projectileColor = MainBodySpriteRenderer.color;
                shoot.AppearanceUpdate();
                _timeForNextAttack = Time.time + _attackDelay;
            }
            //Debug.DrawLine(transform.position, Target.transform.position,Color.red,0.001f);
        }
    }
}