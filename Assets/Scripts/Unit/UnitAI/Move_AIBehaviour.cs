using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_AIBehaviour : AIBehaviour
{
    public override int ChangeState_Enter(UnitAIController controller)
    {
        controller.SetMovePosition();
        return 0;
    }

    public override int ChangeState_Exit(UnitAIController controller)
    {
        return 0;

    }

    public override int Update(UnitAIController controller)
    {
        return 0;
    }

    public override int FixedUpdate(UnitAIController controller)
    {
        if (!controller.IsMoving)
        {
            controller.ChangeState(AIState.Overwatch);
            return 1;
        }

        return 0;
    }
}
