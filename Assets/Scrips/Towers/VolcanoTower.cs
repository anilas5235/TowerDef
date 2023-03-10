using System;
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
            int count = 0;

            do
            {
                count++;
                int rd = Random.Range(0, possiblePathSegments.Count);
                BoxCollider2D col = (BoxCollider2D) possiblePathSegments[rd];
                Vector2 offset = new Vector2( col.size.x/2 * Random.Range(-0.9f, 0.9f), col.size.y/2 * Random.Range(-0.9f, 0.9f));
                print("potentail local Position " +offset + col.transform.parent.gameObject.name);
                targetPosition = col.transform.TransformPoint(offset);
                print("potentail world Position " +targetPosition);

                if (Vector2.Distance(targetPosition,transform.position) > range)
                {
                    //possiblePathSegments.Remove(possiblePathSegments[rd]);
                }
                else
                {
                    done = true;
                }

                if (count > 50)
                {
                    return;
                }

            } while ( !done);

            Instantiate(lavaShoot, targetPosition, quaternion.identity);
        }
    }
}
