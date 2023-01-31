using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private int _currentSpeedMode;
    [SerializeField] private TextMeshProUGUI _speedButtonText;
    public void BuyTower(GameObject Tower)
    {
        Instantiate(Tower, Vector3.zero, quaternion.identity);
    }

    public void SpeedUp()
    {
        _currentSpeedMode++;
        if (_currentSpeedMode > 3)
        { _currentSpeedMode = 0; }
        Time.timeScale = _currentSpeedMode + 1;
        _speedButtonText.text = "Speed: " + Time.timeScale;
    }
}
