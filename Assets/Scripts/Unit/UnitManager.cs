using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    private UnitController[] units;


    public void UpdateUnits()
    {
        units = FindObjectsByType<UnitController>((FindObjectsSortMode)FindObjectsInactive.Exclude);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDebug_Move(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            UpdateUnits();
            foreach (UnitController unitController in units)
            {
                unitController.OnMove();
            }
        }
    }
}
