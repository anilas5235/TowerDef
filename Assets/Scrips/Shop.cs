using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Shop : MonoBehaviour
{
  
    public void BuyTower(GameObject Tower)
    {
        Instantiate(Tower, Vector3.zero, quaternion.identity);
    }
}
