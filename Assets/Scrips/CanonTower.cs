using UnityEngine;
namespace Scrips
{
    public class CanonTower : TowerBase
    {
        private float _attackRadius = 3f, _attackDelay = 1f, _timeForNextAttack; 
        private  int _attackDamage = 1, _multiHit = 1;

        protected override void Start()
        {
            base.Start();
            Indicator.gameObject.transform.localScale = new Vector3(_attackRadius*2, _attackRadius*2, 1);
            _timeForNextAttack = Time.time;
            mainBodySpriteRenderer.color = SetColor();
        }
        
        public Color SetColor()
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

            return currentColor;
        }

        public void UpgradeTower(Vector3 upgrade)
        {
            statsKeeper.UpdateUI();
            upgradeLevel += upgrade;

            _attackRadius += upgrade.x / 2;
            _attackDamage += (int) upgrade.y;
            _multiHit += (int) upgrade.z;
        
            //set Color and Indictor for radius
            mainBodySpriteRenderer.color = SetColor();
            Indicator.gameObject.transform.localScale = new Vector3(_attackRadius*2, _attackRadius*2, 1);
        }
    }
}