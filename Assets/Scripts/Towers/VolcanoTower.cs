using System.Collections.Generic;
using System.Linq;
using Background.Pooling;
using Scrips.Background;
using Scrips.Projectiles;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scrips.Towers
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
            List<Collider2D> possiblePathSegments = Physics2D.OverlapCircleAll(transform.position, attackRadius, pathLayer).ToList();
            Vector3 targetPosition = Vector3.zero;
            bool done = false;
            

            Vector3 GetAPointInBoxCollider(int indexInList)
            {
                BoxCollider2D col = (BoxCollider2D) possiblePathSegments[indexInList];
                Vector2 offset = new Vector2( col.size.x/2 * Random.Range(-0.9f, 0.9f), col.size.y/2 * Random.Range(-1f, 1f));
                Vector3 point = col.transform.TransformPoint(offset);
                return point;
            }

            do
            {
                int count = 0;
                int rd = Random.Range(0, possiblePathSegments.Count);
                
                do
                {
                    count++;
                    targetPosition = GetAPointInBoxCollider(rd);
                    if (Vector2.Distance(targetPosition, transform.position) < attackRadius)
                    {
                        done = true;
                    }else if (count > 3)
                    {
                        possiblePathSegments.Remove(possiblePathSegments[rd]);
                        break;
                    }
                    
                } while (!done);

            } while ( !done);

            LavaShoot shoot = Pool.GetObjectFromPool().GetComponent<LavaShoot>();
            shoot.gameObject.transform.position = targetPosition;
            shoot.storedDamage = _damageLoadPerShoot;
            shoot.Colors = Colors;
            shoot.AppearanceUpdate();
            shoot.pooled = false;
        }
    }
}
