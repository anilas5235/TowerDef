using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Scrips.Background
{
    public class Shop : MonoBehaviour
    {
        private int _currentSpeedMode;
        [SerializeField] private TextMeshProUGUI speedButtonText;
        public void BuyTower(GameObject tower)
        {
            Instantiate(tower, Vector3.zero, quaternion.identity);
        }

        public void SpeedUp()
        {
            _currentSpeedMode++;
            if (_currentSpeedMode > 3)
            { _currentSpeedMode = 0; }
            Time.timeScale = _currentSpeedMode + 1;
            speedButtonText.text = "Speed: " + Time.timeScale;
        }
    }
}
