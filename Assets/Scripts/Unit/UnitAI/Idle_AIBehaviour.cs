using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_AIBehaviour : AIBehaviour
{
    public override int ChangeState_Enter(UnitAIController controller)
    {
        controller.ChangeState(AIState.Move);
        return -1;
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
        return 0;

    }
}
