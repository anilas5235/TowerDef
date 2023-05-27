using UnityEngine;

namespace Background.SplinePath
{
    public class PointBehaviour : MonoBehaviour
    {
        public int index;
        public BaseSplineBuilder Master;
        private Vector3 oldPosition = Vector3.zero;

        private void Reset()
        {
            oldPosition = transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            if (Vector3.Distance(transform.position, oldPosition) > 0.05f)
            {
                Master.TriggerPointMoved(index);
                oldPosition = transform.position;
            }
        }
    }
}