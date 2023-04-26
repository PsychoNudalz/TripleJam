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


    public override bool TakeDamage(float damage, LifeSystem source = null)
    {
        unitController.OnTakeDamage(source);
        return base.TakeDamage(damage, source);
    }

    public override bool TakeDamage(DamageData damageData, LifeSystem source = null)
    {
        unitController.OnTakeDamage(source,damageData);

        return base.TakeDamage(damageData, source);
    }
}
