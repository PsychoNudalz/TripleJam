using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Unit_AI/Behavior/Attack")]

public class Attack_AIBehaviour : AIBehaviour
{
    public override int ChangeState_Enter(UnitAIController controller)
    {
        controller.OnAttack();
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
        if (!controller.Attack.IsAttacking)
        {
            controller.ChangeState(AIState.Reload);
        }
        return 0;
    }

    public override void OnTakeDamage(UnitAIController controller, DamageData damageData, LifeSystem source)
    {
    }
}