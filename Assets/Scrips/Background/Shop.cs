using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Scrips.Background
{
    public class Shop : MonoBehaviour
    {
        public static Shop Instance;
        [SerializeField] private TextMeshProUGUI speedButtonText;
        
        private int _currentSpeedMode;
        private GameObject _currentlyHandledTower;

        private void Awake()
        {
            if (!Instance)
            { Instance = this; }
            else
            { Destroy(this); }
        }

        public void BuyTower(GameObject tower)
        {
            if (_currentlyHandledTower) { return; }
           _currentlyHandledTower = Instantiate(tower, Vector3.zero, quaternion.identity);
        }

        public void TowerHandled()
        {
            _currentlyHandledTower = null;
        }

        public void SpeedUp()
        {
            _currentSpeedMode++;
            if (_currentSpeedMode > 3)
            { _currentSpeedMode = 0; }
            Time.timeScale = _currentSpeedMode + 1f;
            speedButtonText.text = "Speed: " + Time.timeScale;
        }
    }
}
