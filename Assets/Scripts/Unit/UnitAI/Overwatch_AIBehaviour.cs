using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Unit_AI/Behavior/Overwatch")]

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

    public override int UpdateBehaviour(UnitAIController controller)
    {
        return CheckForUnit(controller);

    }

    public override int FixedUpdateBehaviour(UnitAIController controller)
    {
        return CheckForUnit(controller);
    }

    private int CheckForUnit(UnitAIController controller)
    {
        UnitController foundUnit = DetectUnit(controller);
        if (foundUnit)
        {
            controller.SetTarget(foundUnit);
            controller.ChangeState(AIState.Attack);
            return 1;
        }
        // controller.ChangeState(AIState.Move);

        return 0;
    }


}
