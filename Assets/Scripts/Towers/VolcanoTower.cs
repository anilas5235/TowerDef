using System;
using System.Collections.Generic;
using System.Linq;
using Background.Pooling;
using Scrips.Projectiles;
using Scrips.Towers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Towers
{
    public class VolcanoTower : TowerBase
    {
        [SerializeField] private LayerMask pathLayer;
        [SerializeField] private Color[] Colors = new Color[5];
        [SerializeField] private SpriteRenderer lavaTop;
        private LavaShootPool Pool;
        private float _attackDelay = 4;
        private int _damageLoadPerShoot = 5;

        protected override void Start()
        {
            Pool = LavaShootPool.Instance;
            attackRadius = 2f;
            base.Start();
        }

        public override void UpgradeTower(Vector3 upgrade)
        {
            upgradeLevel += upgrade;
            attackRadius += 0.15f * upgrade.x;
            _damageLoadPerShoot += 5 * (int) upgrade.y;
            _attackDelay -= 0.4f * upgrade.z;

            VisualChange();
            indicator.gameObject.transform.localScale = new Vector3(attackRadius*2, attackRadius*2, 1);
        }

        protected override void Attack()
        {
            if (Time.time >= timeForNextAttack)
            {
                ThrowLavaShoot();
                timeForNextAttack = Time.time + _attackDelay;
            }
        }

        protected override void VisualChange()
        {
            lavaTop.color = Colors[(int)upgradeLevel.y];
        }

        private void ThrowLavaShoot()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, attackRadius, pathLayer);
            if (cols.Length < 1)return;
            Vector3 targetPosition = Vector3.zero;
            int index = Random.Range(0, cols.Length);
            int count = 0;
            bool done = false;
            LineRenderer line = (cols[index]).GetComponent<LineRenderer>();
            Vector3[] points = new Vector3[line.positionCount];
            line.GetPositions(points);

            do
            {
                count++;
                int one,two;
                one = Random.Range(0, points.Length);
                do
                {
                    two = Random.Range(0, points.Length);
                } while (two == one);
            
                targetPosition = Vector3.Lerp(points[one], points[two], Random.Range(0, 1f));
                targetPosition.z = 0;
                targetPosition += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0); 
            
                if (Vector3.Distance(transform.position,targetPosition)< attackRadius+0.1f) done = true;

                if (count > 100) done = true;

            } while (!done);

            LavaShoot shoot = Pool.GetObjectFromPool().GetComponent<LavaShoot>();
            shoot.gameObject.transform.position = targetPosition;
            shoot.storedDamage = _damageLoadPerShoot;
            shoot.Colors = Colors;
            shoot.AppearanceUpdate();
            shoot.pooled = false;
        }

        private void OnDrawGizmos()
        {
            int RaysToShoot = 36;
            float angle = 0;
            for (int i = 0; i < RaysToShoot; i++)
            {
                float x = Mathf.Sin(angle);
                float y = Mathf.Cos(angle);
                angle += 2 * Mathf.PI / RaysToShoot;

                Vector3 dir = new Vector3( x,  y, 0);
                Gizmos.DrawLine(transform.position,   transform.position+ dir  * attackRadius);
            }

        }
    }
}
