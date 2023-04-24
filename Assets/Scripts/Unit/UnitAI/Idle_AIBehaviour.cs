using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unit_AI/Behavior/Idle")]

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

    public override int UpdateBehaviour(UnitAIController controller)
    {
        return 0;

    }

    public override int FixedUpdateBehaviour(UnitAIController controller)
    {
        return 0;

    }
}
