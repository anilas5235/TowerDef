using System.Collections.Generic;
using UnityEngine;

namespace Scrips.Towers
{
    public class BellowTower : TowerBase
    {
        [SerializeField] private PolygonCollider2D pushArea;

        private Vector2[] _pointsForPushArea = new Vector2[9];
        private ContactFilter2D filter;
        private float _angleForPushArea = 90,_attackDelay = 5f;
        private int _throwBackStrength = 2;
        protected override void Start()
        {
            filter.NoFilter();
            filter.SetLayerMask(enemyLayer);
            filter.SetDepth(-50,50);
            attackRadius = 2f;
            CalculatePointsForPushArea();
            pushArea.enabled = false;
            base.Start();
        }

        public override void UpgradeTower(Vector3 upgrade)
        {
            
        }

        protected override void Attack()
        {
            pushArea.enabled = true;
            Vector3 targetDirection = Target.transform.position - transform.position;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg % 360 - 90;
            BarrelPivotGameObject.transform.localRotation = Quaternion.Euler(0,0,angle);

            if (Time.time >= timeForNextAttack)
            {
                List<Collider2D> targets = new List<Collider2D>();
                pushArea.OverlapCollider(filter, targets);

                foreach (Collider2D target in targets)
                {
                    target.GetComponent<Enemy>().ThrowBack(_throwBackStrength,
                        (target.transform.position - transform.position).normalized * _throwBackStrength);
                }
                
                timeForNextAttack = Time.time + _attackDelay;
            }
        }

        protected override void VisualChange()
        {
            
        }

        private void CalculatePointsForPushArea()
        {
            _pointsForPushArea[0] =  Vector2.zero;
            float currentAngle = 90+_angleForPushArea/2;
            for (int i = 0; i < 7; i++)
            {
                _pointsForPushArea[i + 1] = (DegreeToVector2(currentAngle)*attackRadius);
                currentAngle -= _angleForPushArea/6;
            }
            _pointsForPushArea[8] = Vector2.zero;
            pushArea.SetPath(0,_pointsForPushArea);
        }

        private Vector2 DegreeToVector2(float degree)
        {
            float radian = degree * Mathf.Deg2Rad;
            Vector2 vector = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
            return vector;
        }
    }
}
