using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace UIScripts.ShopUi
{
    public class Shop : MonoBehaviour
    {
        public static Shop Instance;
        [SerializeField] private TextMeshProUGUI speedButtonText;

        private int currentSpeedMode = 1;
        public int CurrentSpeedMode
        {
            get => currentSpeedMode;
        }
        public float priceMultiplier = 1f;
        private GameObject _currentlyHandledTower;

        private void Awake()
        {
            if (!Instance) Instance = this; 
            else Destroy(this); 
        }

        public void BuyTower(GameObject tower)
        {
            if (_currentlyHandledTower)  return; 
           _currentlyHandledTower = Instantiate(tower, Vector3.zero, quaternion.identity);
        }

        public void TowerHandled()
        {
            _currentlyHandledTower = null;
        }

        public void SpeedUp()
        {
            currentSpeedMode++;
            if (currentSpeedMode > 3)
            { currentSpeedMode = 1; }
            Time.timeScale = currentSpeedMode ;
            speedButtonText.text = "Speed: " + Time.timeScale;
        }
    }
}
