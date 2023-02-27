using UnityEngine;

namespace Scrips
{
    public class Boom : MonoBehaviour
    {
        public void DestroySelf()
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
