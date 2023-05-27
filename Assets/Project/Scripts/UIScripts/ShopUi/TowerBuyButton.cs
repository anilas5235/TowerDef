using Background.Keeper;
using Scrips.Towers;
using TMPro;
using Towers;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts.ShopUi
{


    public class TowerBuyButton : MonoBehaviour
    {
        [SerializeField] private GameObject tower;

        private TowerData _myTowerData;
        private Button _myButton;
        private TextMeshProUGUI _buttonText;

        private void Start()
        {
            _myTowerData = tower.GetComponent<TowerBase>().towerData;

            _myButton = gameObject.GetComponent<Button>();
            _buttonText = _myButton.GetComponentInChildren<TextMeshProUGUI>();
            _buttonText.text = $"{_myTowerData.placingCosts * Shop.Instance.priceMultiplier}";
        }
        private void OnGUI()
        {
            _myButton.interactable = StatsKeeper.Instance.Money >= _myTowerData.placingCosts;
        }

        public void SpawnTower()
        {
            Shop.Instance.BuyTower(tower);
        }
    }
}