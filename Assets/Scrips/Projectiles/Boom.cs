using UnityEngine;

namespace Scrips.Projectiles
{
    public class Boom : MonoBehaviour
    {
        public void DestroySelf()
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
