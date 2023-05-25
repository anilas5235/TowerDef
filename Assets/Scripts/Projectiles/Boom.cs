using UnityEngine;

namespace Projectiles
{
    public class Boom : MonoBehaviour
    {
        public void DestroySelf()
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
