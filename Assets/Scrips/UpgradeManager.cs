using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    private Camera _cam;

    [SerializeField] private LayerMask blocking;
    private GameObject TowerUiWindow, Stat0, Stat1, Stat2;
    private TextMeshProUGUI Stat0name, Stat0cost,  Stat1name, Stat1cost, Stat2name, Stat2cost, Towername;//UI für Upgrade window texts
    private Tower TowerData;
    private GameObject RadiusIndicator;
    
    private Button Stat0button, Stat1button, Stat2button;
    private Vector3 UpgradeLevel;
    private StatsKeeper StatsKeeper;
    private TowerBasic currentTowerScript;

    private bool active, mouseoverUI;

    private void Start()
    {
        _cam = Camera.main;
        
        TowerUiWindow = GameObject.Find("TowerUI");
        StatsKeeper = StatsKeeper.Instance;
        
        Stat0 = TowerUiWindow.gameObject.transform.GetChild(2).gameObject;
        Stat1 = TowerUiWindow.gameObject.transform.GetChild(3).gameObject;
        Stat2 = TowerUiWindow.gameObject.transform.GetChild(4).gameObject;
        
        Towername = TowerUiWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        Stat0name = Stat0.GetComponent<TextMeshProUGUI>();
        Stat0cost = Stat0.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Stat0button = Stat0.transform.GetChild(1).GetComponent<Button>();
        Stat1name = Stat1.GetComponent<TextMeshProUGUI>();
        Stat1cost = Stat1.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Stat1button = Stat1.transform.GetChild(1).GetComponent<Button>();
        Stat2name = Stat2.GetComponent<TextMeshProUGUI>();
        Stat2cost = Stat2.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Stat2button = Stat2.transform.GetChild(1).GetComponent<Button>();
        
        TowerUiWindow.SetActive(false); active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !mouseoverUI)
        {
            Vector3 mousePosition = _cam.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Collider2D[] cols = Physics2D.OverlapCircleAll(mousePosition, 0.1f, blocking);
            

            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject.CompareTag("Tower") && cols[i].GetComponent<TowerBasic>().placed)
                {
                    if (currentTowerScript?.selected == true && currentTowerScript != cols[i].GetComponent<TowerBasic>() ) { DeselectTower(); }
                    
                    currentTowerScript = cols[i].GetComponent<TowerBasic>(); print("pass");
                    SelectTower();
                    
                    if (mousePosition.x > 1.5f) //place UI left or right
                    { Vector3 T = TowerUiWindow.transform.position; T.x = -4; TowerUiWindow.transform.position = T; }
                    else { Vector3 T = TowerUiWindow.transform.position; T.x = 3.25f+1.5f; TowerUiWindow.transform.position = T; }

                    break;
                }

                currentTowerScript = null;
            }

            if ((cols.Length < 1 || !currentTowerScript ) && active) { DeselectTower(); }
        }
    }

    private void SetUiWindowText()
    {
        UpgradeLevel = currentTowerScript.upgradeLevel;

        //set the texts
        Towername.text = TowerData.Towername;

        if (UpgradeLevel.x < TowerData.attackradiusUpgradeCosts.Length)
        {
            Stat0name.text = TowerData.statNames[0] + " :\n lvl " + ((int)UpgradeLevel.x + 1);
            Stat0cost.text = "Cost to Upgrade : \n" + TowerData.attackradiusUpgradeCosts[(int)UpgradeLevel.x];
            Stat0button.gameObject.SetActive( true);
        }
        else
        {
            Stat0name.text = TowerData.statNames[0] + " :\n lvl Max";
            Stat0cost.text = "";
            Stat0button.gameObject.SetActive( false);
        }

        if (UpgradeLevel.y < TowerData.attackdamageUpgradeCosts.Length)
        {
            Stat1name.text = TowerData.statNames[1] + " : lvl " + ((int)UpgradeLevel.y + 1);
            Stat1cost.text = "Cost to Upgrade : \n" + TowerData.attackradiusUpgradeCosts[(int)UpgradeLevel.y];
            Stat1button.gameObject.SetActive(true);
        }
        else
        {
            Stat1name.text = TowerData.statNames[1] + " :\n lvl Max";
            Stat1cost.text = "";
            Stat1button.gameObject.SetActive(false);
        }

        if (UpgradeLevel.z < TowerData.multiHitUpgradeCosts.Length)
        {
             Stat2name.text = TowerData.statNames[2]+" :\n lvl "+((int)UpgradeLevel.z +1);
             Stat2cost.text = "Cost to Upgrade : \n" + TowerData.attackradiusUpgradeCosts[(int)UpgradeLevel.z] ;
             Stat2button.gameObject.SetActive(true);
        }
        else
        {
            Stat2name.text = TowerData.statNames[2]+ " :\n lvl Max";
            Stat2cost.text = "";
            Stat2button.gameObject.SetActive(false);
        }
    }

    public void UpgradeStat(int index)
    {
        int cost =0;
        switch (index)
        {
            case 0: cost = TowerData.attackradiusUpgradeCosts[(int)UpgradeLevel.x]; break;
            case 1: cost = TowerData.attackradiusUpgradeCosts[(int)UpgradeLevel.y]; break;
            case 2: cost = TowerData.attackradiusUpgradeCosts[(int)UpgradeLevel.z]; break;
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
