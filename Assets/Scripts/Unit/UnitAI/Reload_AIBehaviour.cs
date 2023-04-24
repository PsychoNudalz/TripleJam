using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Unit_AI/Behavior/Reload")]

public class Reload_AIBehaviour : AIBehaviour
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
        return 0;
    }

    public override int FixedUpdateBehaviour(UnitAIController controller)
    {
        return 0;
    }
}