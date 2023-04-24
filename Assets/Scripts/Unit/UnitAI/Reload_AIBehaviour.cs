using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Unit_AI/Behavior/Reload")]

public class Reload_AIBehaviour : AIBehaviour
{
    [SerializeField]
    private float reloadTime = 0;
    public override int ChangeState_Enter(UnitAIController controller)
    {
        reloadTime = controller.AttackCooldown;
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
        reloadTime -= Time.deltaTime;
        if (reloadTime < 0)
        {
            controller.ChangeState(AIState.Overwatch);
            return 1;
        }
        return 0;
    }
}