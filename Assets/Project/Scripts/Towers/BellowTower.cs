using System.Collections.Generic;
using Background.Pooling;
using Projectiles;
using Scrips;
using Scrips.Towers;
using UnityEngine;

namespace Towers
{
    public class BellowTower : TowerBase
    {
        [SerializeField] private Transform barrelTip;

        private ContactFilter2D _filter2D;
        private float _attackDelay = 5f, _airBladeSize = 0.3f;
        private float _throwBackStrength = 2;
        protected override void Start()
        {
            _filter2D.NoFilter();
            _filter2D.SetLayerMask(enemyLayer);
            _filter2D.SetDepth(-50,50);
            attackRadius = 2f;
            base.Start();
        }

        public override void UpgradeTower(Vector3 upgrade)
        {
            upgradeLevel += upgrade;
            _airBladeSize += 0.04f * upgrade.x;
            _throwBackStrength +=  0.6f * upgrade.y;
            _attackDelay -= 0.6f * upgrade.z;
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
                timeForNextAttack = Time.time + _attackDelay;
                AirBlade current = AirBladePool.Instance.GetObjectFromPool().GetComponentInChildren<AirBlade>();
                current.transform.position = barrelTip.position;
                current.transform.localRotation = BarrelPivotGameObject.transform.localRotation;
                current.direction = (barrelTip.position - transform.position).normalized;
                current.StartDeathTimer(1f+ _airBladeSize);
                current.sizeMultiplier = _airBladeSize;
                current.speed = _throwBackStrength;
            }
        }

        protected override void VisualChange()
        {
            MainBodySpriteRenderer.color = ColorSequence((int) upgradeLevel.x);
        }

        private Vector2 DegreeToVector2(float degree)
        {
            float radian = degree * Mathf.Deg2Rad;
            Vector2 vector = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
            return vector;
        }
    }
}
