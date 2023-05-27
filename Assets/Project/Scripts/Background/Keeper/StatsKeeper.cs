using TMPro;
using UIScripts;
using UnityEngine;

namespace Background.Keeper
{
    public class StatsKeeper : MonoBehaviour
    {
        public static StatsKeeper Instance;
    
        private int hp = 100, money =600;
        [SerializeField] private TextMeshProUGUI HPUI,MoneyUI;

        public int Money
        {
            get => money;
            set { money = value; UpdateUI(); }
        }
        public int Hp
        {
            get => hp;
            set { hp = value; UpdateUI();
                if (hp < 1) UIMaster.Instance.ChangeUIStateWithIndex(4);
            }
        }



        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            else { Destroy(this); }
        }

        private void Start()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if(HPUI )  HPUI.text = $"{hp}" ;
            if(MoneyUI)  MoneyUI.text = $"{Money}" ;
        }
    }
}
