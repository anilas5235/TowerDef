using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class StatsKeeper : MonoBehaviour
{
    public int hp = 100;
    private TextMeshProUGUI HP;

    private void Start()
    {
        HP = GameObject.Find("Hp_Anzeige").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        HP.text = "leben :" + hp;
    }
}
