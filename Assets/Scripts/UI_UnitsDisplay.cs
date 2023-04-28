using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_UnitsDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] textFields;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI(UnitController[] units)
    {
        for (int i = 0; i < Mathf.Min(units.Length,textFields.Length); i++)
        {
            textFields[i].text = units[i].UnitDisplay();
        }
    }
}
