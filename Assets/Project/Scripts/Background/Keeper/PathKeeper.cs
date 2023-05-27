using UnityEngine;

namespace Background.Keeper
{
    public class PathKeeper : MonoBehaviour
    {
        public static PathKeeper Instance;
        public PathPointSave Save;

        public Vector3[] PathPoints;
        
        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            else { Destroy(this); }

            if (Save != null)
            {
                PathPoints = Save.Points;
            }
            else
            {
                Debug.Log($"No PathPointSave assigned");
            }
        }
    }
}
