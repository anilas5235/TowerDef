using Scrips;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    private Camera _cam;

    [SerializeField] private LayerMask towerLayer;
    private GameObject TowerUiWindow, stat0, stat1, stat2;
    private TextMeshProUGUI stat0name, stat0cost,  stat1name, stat1cost, stat2name, stat2cost, Towername;//UI für Upgrade window texts
    private Image stat0ButtonColor, stat1ButtonColor, stat2ButtonColor;
    private TowerData TowerData;
    private GameObject RadiusIndicator;
    
    private Button stat0button, stat1button, stat2button;
    private Vector3 UpgradeLevel;
    private StatsKeeper StatsKeeper;
    private TowerBase currentTowerScript;

    private bool active, mouseoverUI;

    private void Start()
    {
        _cam = Camera.main;
        
        TowerUiWindow = GameObject.Find("TowerUI");
        StatsKeeper = StatsKeeper.Instance;
        
        stat0 = TowerUiWindow.gameObject.transform.GetChild(2).gameObject;
        stat1 = TowerUiWindow.gameObject.transform.GetChild(3).gameObject;
        stat2 = TowerUiWindow.gameObject.transform.GetChild(4).gameObject;
        
        Towername = TowerUiWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        stat0name = stat0.GetComponent<TextMeshProUGUI>();
        stat0cost = stat0.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        stat0button = stat0.transform.GetChild(1).GetComponent<Button>();
        stat0ButtonColor = stat0.transform.GetChild(1).GetComponent<Image>();
        stat1name = stat1.GetComponent<TextMeshProUGUI>();
        stat1cost = stat1.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        stat1button = stat1.transform.GetChild(1).GetComponent<Button>();
        stat1ButtonColor = stat1.transform.GetChild(1).GetComponent<Image>();
        stat2name = stat2.GetComponent<TextMeshProUGUI>();
        stat2cost = stat2.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        stat2button = stat2.transform.GetChild(1).GetComponent<Button>();
        stat2ButtonColor = stat2.transform.GetChild(1).GetComponent<Image>();
        
        TowerUiWindow.SetActive(false); active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !mouseoverUI)
        {
            Vector3 mousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Collider2D[] cols = Physics2D.OverlapCircleAll(mousePosition, 0.1f, towerLayer);
            

            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject.CompareTag("Tower") && cols[i].GetComponent<TowerBase>().placed)
                {
                    if (currentTowerScript?.selected == true && currentTowerScript != cols[i].GetComponent<TowerBase>() ) { DeselectTower(); }
                    
                    currentTowerScript = cols[i].GetComponent<TowerBase>();
                    SelectTower();
                    
                    if (mousePosition.x > 1.5f) //place UI left or right
                    { Vector3 T = TowerUiWindow.transform.position; T.x = -438 *0.01111111f ; TowerUiWindow.transform.position = T; }
                    else { Vector3 T = TowerUiWindow.transform.position; T.x = 325 *0.01111111f ; TowerUiWindow.transform.position = T; }

                    break;
                }

                currentTowerScript = null;
            }

            if ((cols.Length < 1 || !currentTowerScript ) && active) { DeselectTower(); }
        }
        
        if(active){ ButtonUpdate();}
    }

    private void SetUiWindowText()
    {
        UpgradeLevel = currentTowerScript.upgradeLevel;

        //set the texts
        Towername.text = TowerData.towerName;

        if (UpgradeLevel.x < TowerData.upgradeCostsSlot0.Length)
        {
            stat0name.text = TowerData.statNames[0] + " :\n lvl " + ((int)UpgradeLevel.x + 1);
            stat0cost.text = "Cost to Upgrade : \n" + TowerData.upgradeCostsSlot0[(int)UpgradeLevel.x];
            stat0button.gameObject.SetActive( true);
        }else
        {
            stat0name.text = TowerData.statNames[0] + " :\n lvl Max";
            stat0cost.text = "";
            stat0button.gameObject.SetActive( false);
        }

        if (UpgradeLevel.y < TowerData.upgradeCostsSlot1.Length)
        {
            stat1name.text = TowerData.statNames[1] + " : lvl " + ((int)UpgradeLevel.y + 1);
            stat1cost.text = "Cost to Upgrade : \n" + TowerData.upgradeCostsSlot1[(int)UpgradeLevel.y];
            stat1button.gameObject.SetActive(true);
        }else
        {
            stat1name.text = TowerData.statNames[1] + " :\n lvl Max";
            stat1cost.text = "";
            stat1button.gameObject.SetActive(false);
        }

        if (UpgradeLevel.z < TowerData.upgradeCostsSlot2.Length)
        {
             stat2name.text = TowerData.statNames[2]+" :\n lvl "+((int)UpgradeLevel.z +1);
             stat2cost.text = "Cost to Upgrade : \n" + TowerData.upgradeCostsSlot2[(int)UpgradeLevel.z] ;
             stat2button.gameObject.SetActive(true);
        }else
        {
            stat2name.text = TowerData.statNames[2]+ " :\n lvl Max";
            stat2cost.text = "";
            stat2button.gameObject.SetActive(false);
        }
    }

    private void ButtonUpdate()
    {
        var green = new Color(23 / 255f, 130 / 255f, 20 / 255f);
        if (UpgradeLevel.x < TowerData.upgradeCostsSlot0.Length)
        { stat0ButtonColor.color = StatsKeeper.Money < TowerData.upgradeCostsSlot0[(int)UpgradeLevel.x] ? Color.red : green; }
        if(UpgradeLevel.y < TowerData.upgradeCostsSlot1.Length)
        {stat1ButtonColor.color = StatsKeeper.Money < TowerData.upgradeCostsSlot1[(int)UpgradeLevel.y] ? Color.red : green;}    
        if(UpgradeLevel.z <  TowerData.upgradeCostsSlot2.Length)
        {stat2ButtonColor.color = StatsKeeper.Money < TowerData.upgradeCostsSlot2[(int)UpgradeLevel.z] ? Color.red : green;}
    }

    public void UpgradeStat(int index)
    {
        int cost =0;
        switch (index)
        {
            case 0: cost = TowerData.upgradeCostsSlot0[(int)UpgradeLevel.x]; break;
            case 1: cost = TowerData.upgradeCostsSlot1[(int)UpgradeLevel.y]; break;
            case 2: cost = TowerData.upgradeCostsSlot2[(int)UpgradeLevel.z]; break;
            default: print(" For this Upgradeindex "+index+ " is not defined a function"); return;
                
        }
         
        if(cost > StatsKeeper.Money){print("Upgrade failed, not enough Money");  return;}
        StatsKeeper.Money -= cost;
        
        Vector3 uVector= Vector3.zero;
        switch (index)
        {
            case 0: uVector = new Vector3(1, 0, 0); break;
            case 1: uVector = new Vector3(0, 1, 0); break;
            case 2: uVector = new Vector3(0, 0, 1);break;
        }

        currentTowerScript.UpgradeTower(uVector);
        SetUiWindowText();
    }

    public void HoverOverUI(bool val)
    {
        mouseoverUI = val;
    }

    private void SelectTower()
    {
        TowerData = currentTowerScript.TowerData;
        currentTowerScript.Indicator.enabled = true;
        TowerUiWindow.SetActive(true);
        active = true;
        SetUiWindowText();
        currentTowerScript.selected = true;
    }

    private void DeselectTower()
    {
        currentTowerScript.selected = false;
        currentTowerScript.Indicator.enabled = false;
        TowerUiWindow.SetActive(false);
        active = false;
        currentTowerScript = null;
    }
}
