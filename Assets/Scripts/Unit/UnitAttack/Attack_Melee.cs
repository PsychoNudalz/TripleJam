using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Melee : UnitAttack
{
    public override void OnAttack_Enter(UnitController target)
    {
        base.OnAttack_Enter(target);
    }

    public override void OnAttack_Action(UnitController target)
    {
        base.OnAttack_Action(target);
        target.ls.TakeDamage(new DamageData(transform.position,2f,damage));
    }

    public override void OnAttack_Exit(UnitController target)
    {
        base.OnAttack_Exit(target);
    }
    
    
}