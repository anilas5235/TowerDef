using Scrips.Background;
using Scrips.Towers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Background
{
    public class UpgradeManager : MonoBehaviour
    {
        [SerializeField] private LayerMask towerLayer;
        [SerializeField] private GameObject towerUiWindow;

        [Header("Text-fields")]
        [SerializeField] private TextMeshProUGUI[] statNames;
        [SerializeField] private TextMeshProUGUI[]statCosts;
        [SerializeField] private TextMeshProUGUI towerName, extraButtonText;
        
        [Header("Buttons")]
        [SerializeField] private Button[] statButtons;
        [SerializeField] private Image[] statButtonColors;
        [SerializeField] private Button extraButton;
        
        private Camera _cam;
        private TowerData _towerData;
        private StatsKeeper _statsKeeper;
        private TowerBase _currentTowerScript;
        private Vector3 _upgradeLevel;
        private bool _active, _mouseoverUI;

        private void Start()
        {
            _cam = Camera.main;
            _statsKeeper = StatsKeeper.Instance;
            towerUiWindow.SetActive(false); _active = false;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !_mouseoverUI)
            {
                Vector3 mousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                Collider2D[] cols = Physics2D.OverlapCircleAll(mousePosition, 0.1f, towerLayer);

                foreach (var col in cols)
                {
                    if (col.gameObject.CompareTag("Tower") && col.GetComponent<TowerBase>().placed)
                    {
                        TowerBase newTowerBase = col.GetComponent<TowerBase>();
                        if (_currentTowerScript && _currentTowerScript != newTowerBase ) { DeselectTower(); }

                        _currentTowerScript = newTowerBase;
                        SelectTower();
                        
                        Vector3 T = towerUiWindow.transform.localPosition;
                        T.x = mousePosition.x > 1.5f ? -710f : 325f; //place UI left or right
                        towerUiWindow.transform.localPosition = T;

                        break;
                    }

                    _currentTowerScript = null;
                }

                if ((cols.Length < 1 || !_currentTowerScript ) && _active) { DeselectTower(); }
            }
        
            if(_active){ ButtonUpdate();}
        }

        private void SetUiWindowText()
        {
            int i = 0;
            void SetTextForStat( bool furtherUpgradable, int upgradeLevelOfState, int[] costForNextUpgrades)
            {
                if (furtherUpgradable)
                {
                    statNames[i].text = _towerData.statNames[i] + " :\n lvl " + (upgradeLevelOfState + 1);
                    statCosts[i].text = "Cost to Upgrade : \n" + costForNextUpgrades[upgradeLevelOfState];
                    statButtons[i].gameObject.SetActive( true);
                }else
                {
                    statNames[i].text = _towerData.statNames[i] + " :\n lvl Max";
                    statCosts[i].text = "";
                    statButtons[i].gameObject.SetActive( false);
                }
                i++;
            }
            _upgradeLevel = _currentTowerScript.upgradeLevel;

            //set the texts
            towerName.text = _towerData.towerName;
            
            SetTextForStat(_upgradeLevel.x < _towerData.upgradeCostsSlot0.Length,(int)_upgradeLevel.x,_towerData.upgradeCostsSlot0);
            
            SetTextForStat(_upgradeLevel.y < _towerData.upgradeCostsSlot1.Length,(int)_upgradeLevel.y,_towerData.upgradeCostsSlot1);
            
            SetTextForStat(_upgradeLevel.z < _towerData.upgradeCostsSlot2.Length,(int)_upgradeLevel.z,_towerData.upgradeCostsSlot2);

            
            switch (_towerData.id)
            {
                case 7: extraButton.gameObject.SetActive(true);
                    extraButtonText.text = "Set Target "; 
                    break;
                default: extraButton.gameObject.SetActive(false); break;
            }
        }

        private void ButtonUpdate()
        {
            var green = new Color(23 / 255f, 130 / 255f, 20 / 255f);
            if (_upgradeLevel.x < _towerData.upgradeCostsSlot0.Length)
            { statButtonColors[0].color = _statsKeeper.Money < _towerData.upgradeCostsSlot0[(int)_upgradeLevel.x] ? Color.red : green;}
            if(_upgradeLevel.y < _towerData.upgradeCostsSlot1.Length)
            {statButtonColors[1].color = _statsKeeper.Money < _towerData.upgradeCostsSlot1[(int)_upgradeLevel.y] ? Color.red : green;}    
            if(_upgradeLevel.z <  _towerData.upgradeCostsSlot2.Length)
            {statButtonColors[2].color = _statsKeeper.Money < _towerData.upgradeCostsSlot2[(int)_upgradeLevel.z] ? Color.red : green;}
        }

        public void UpgradeStat(int index)
        {
            int cost =0;
            switch (index)
            {
                case 0: if (_upgradeLevel.x > _towerData.upgradeCostsSlot0.Length) {return; }
                    cost = _towerData.upgradeCostsSlot0[(int)_upgradeLevel.x]; break;
                case 1: if (_upgradeLevel.y > _towerData.upgradeCostsSlot1.Length) {return; }
                    cost = _towerData.upgradeCostsSlot1[(int)_upgradeLevel.y]; break;
                case 2: if (_upgradeLevel.z > _towerData.upgradeCostsSlot2.Length) {return; }
                    cost = _towerData.upgradeCostsSlot2[(int)_upgradeLevel.z]; break;
                default: print(" For this Upgrade index "+index+ " is not defined a function"); 
                    return;
            }
         
            if(cost > _statsKeeper.Money){print("Upgrade failed, not enough Money");  return;}
            _statsKeeper.Money -= cost;
        
            Vector3 uVector= Vector3.zero;
            switch (index)
            {
                case 0: uVector = new Vector3(1, 0, 0); break;
                case 1: uVector = new Vector3(0, 1, 0); break;
                case 2: uVector = new Vector3(0, 0, 1);break;
            }

            _currentTowerScript.UpgradeTower(uVector);
            SetUiWindowText();
        }

        public void HoverOverUI(bool val)
        {
            _mouseoverUI = val;
        }

        private void SelectTower()
        {
            _towerData = _currentTowerScript.towerData;
            _currentTowerScript.indicator.enabled = true;
            towerUiWindow.SetActive(true);
            _active = true;
            SetUiWindowText();
        }

        private void DeselectTower()
        {
            _currentTowerScript.indicator.enabled = false;
            towerUiWindow.SetActive(false);
            _active = false;
            _currentTowerScript = null;
            _towerData = null;
        }

        public void TowerExtraFunction()
        {
            switch (_towerData.id)
            {
                case 7: _currentTowerScript.gameObject.GetComponent<ArtilleryTower>().ChangingTargetPosition();
                    DeselectTower(); _mouseoverUI = false;  break;
            }
        }
    }
}
