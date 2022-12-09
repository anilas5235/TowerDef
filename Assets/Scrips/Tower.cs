using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class Tower : ScriptableObject
{
    public int placingCosts;
    public int[] attackradiusUpgradeCosts;
    public int[] attackdamageUpgradeCosts;
    public int[] multiHitUpgradeCosts;
    public string[] statNames;
    public string Towername;
}
