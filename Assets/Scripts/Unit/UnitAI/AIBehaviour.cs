using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Unit_AI/Behavior/Base")]
public abstract class AIBehaviour: ScriptableObject
{
    // return values:
    // 0: okay
    // 1: changed state
    // -1: error
    public abstract int ChangeState_Enter(UnitAIController controller);

    public abstract int ChangeState_Exit(UnitAIController controller);

    public abstract int UpdateBehaviour(UnitAIController controller);

    public abstract int FixedUpdateBehaviour(UnitAIController controller);

    public static UnitController DetectUnit(UnitAIController controller, float range)
    {

        Collider[] colliders = Physics.OverlapSphere(controller.Position, range, controller.LayerMask);
        UnitController unit;
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out unit))
            {
                if (unit.IsHostile(controller.UnitController))
                {
                    return unit;
                }
            }
        }

        return null;
    }
    private int CheckForUnit(UnitAIController controller,float range)
    {
        UnitController foundUnit = DetectUnit(controller,range);
        if (foundUnit)
        {
            controller.SetTarget(foundUnit);
            controller.ChangeState(AIState.Attack);
            return 1;
        }
        // controller.ChangeState(AIState.Move);

        return 0;
    }


    private Vector3 GetRetreatDirection(UnitAIController controller, Vector3 damageSource, float range)
    {
        return new Vector3();
    }

    public abstract void OnTakeDamage(UnitAIController controller, DamageData damageData, LifeSystem source);

    protected void RetreatFromDamage(UnitController controller, DamageData damageData)
    {
        
    }
    
}
