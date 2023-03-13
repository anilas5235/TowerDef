using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scrips.Towers
{
    public class VolcanoTower : MonoBehaviour
    {
        private float range = 2f;

        [SerializeField] private GameObject lavaShoot;
        [SerializeField] private LayerMask pathLayer;

        private void Start()
        {
            for (int i = 0; i < 1000; i++)
            {
               ThrowLavaShoot(); 
            }
            
        }

        private void ThrowLavaShoot()
        {
            List<Collider2D> possiblePathSegments = Physics2D.OverlapCircleAll(transform.position, range, pathLayer).ToList();
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
                    if (Vector2.Distance(targetPosition, transform.position) < range)
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
