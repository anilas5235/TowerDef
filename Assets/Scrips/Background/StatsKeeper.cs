using TMPro;
using UnityEngine;

namespace Scrips.Background
{
    public class StatsKeeper : MonoBehaviour
    {
        public static StatsKeeper Instance;
    
        private int hp = 100, money =60000;
        private TextMeshProUGUI HPUI,MoneyUI;

        public int Money
        {
            get => money;
            set { money = value; UpdateUI(); }
        }
        public int Hp
        {
            get => hp;
            set { hp = value; UpdateUI(); }
        }



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

        private void UpdateUI()
        {
            HPUI.text = "leben :" + hp;
            MoneyUI.text = "Money :" + Money;
        }
    }
}
