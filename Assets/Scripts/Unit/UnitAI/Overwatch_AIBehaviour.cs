using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overwatch_AIBehaviour : AIBehaviour
{
    public override int ChangeState_Enter(UnitAIController controller)
    {
        return 0;
    }

    public override int ChangeState_Exit(UnitAIController controller)
    {
        return 0;
    }

    public override int Update(UnitAIController controller)
    {
        return CheckForUnit(controller);

    }

    public override int FixedUpdate(UnitAIController controller)
    {
        return CheckForUnit(controller);
    }

    private int CheckForUnit(UnitAIController controller)
    {
        UnitController foundUnit = DetectUnit(controller);
        if (foundUnit)
        {
            controller.ChangeState(AIState.Attack);
            return 1;
        }
        controller.ChangeState(AIState.Move);

        return 0;
    }

    public UnitController DetectUnit(UnitAIController controller)
    {

        Collider[] colliders = Physics.OverlapSphere(controller.Position, controller.AttackRange, controller.LayerMask);
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
}