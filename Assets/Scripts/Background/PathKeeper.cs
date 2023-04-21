using UnityEngine;

namespace Scrips.Background
{
    public class PathKeeper : MonoBehaviour
    {
        public static PathKeeper Instance;
        public GameObject[] PathPoints;
        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            else { Destroy(this); }
    
            PathPoints = GameObject.FindGameObjectsWithTag("PathPoint");
        }
    }
}
