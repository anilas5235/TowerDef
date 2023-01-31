using TMPro;
using UnityEngine;

namespace Scrips.Background
{
    public class StatsKeeper : MonoBehaviour
    {
        public static StatsKeeper Instance;
    
        public int hp = 100, Money =100;
        private TextMeshProUGUI HPUI,MoneyUI;


        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            else { Destroy(this); }
        }

        private void Start()
        {
            HPUI = GameObject.Find("Hp_Anzeige").GetComponent<TextMeshProUGUI>();
            MoneyUI = GameObject.Find("money_Anzeige").GetComponent<TextMeshProUGUI>();
            UpdateUI();
        }

        public void UpdateUI()
        {
            HPUI.text = "leben :" + hp;
            MoneyUI.text = "Money :" + Money;
        }
    }
}
