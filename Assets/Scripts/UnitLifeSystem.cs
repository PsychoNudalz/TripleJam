using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLifeSystem : LifeSystem
{
    [Header("Unit")]
    [SerializeField]
    private UnitController unitController;

    private void Awake()
    {
        if (!unitController)
        {
            unitController = GetComponent<UnitController>();
        }
    }


    public override bool TakeDamage(float f)
    {
        unitController.OnTakeDamage();
        return base.TakeDamage(f);
    }
}
