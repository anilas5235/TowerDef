using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scrips.Towers
{
    public class VolcanoTower : TowerBase
    {
        [SerializeField] private GameObject lavaShoot;
        [SerializeField] private LayerMask pathLayer;
        
        private float _attackDelay = 3;

        protected override void Start()
        {
            attackRadius = 2f;
            base.Start();
        }

        public override void UpgradeTower(Vector3 upgrade)
        {
            throw new System.NotImplementedException();
        }

        protected override void Attack()
        {
            if (Time.time >= timeForNextAttack && Physics2D.OverlapCircle(transform.position, attackRadius, enemyLayer))
            {
                ThrowLavaShoot();
                timeForNextAttack = Time.time + _attackDelay;
            }
        }

        protected override void VisualChange()
        {
            throw new System.NotImplementedException();
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

            
            Instantiate(lavaShoot, targetPosition, quaternion.identity);
        }
    }
}
