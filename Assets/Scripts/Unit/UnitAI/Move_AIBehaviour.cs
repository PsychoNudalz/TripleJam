using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Unit_AI/Behavior/Move")]

public class Move_AIBehaviour : AIBehaviour
{
    [FormerlySerializedAs("OverwatchWhenInRange")]
    [SerializeField]
    private bool overwatchWhenInRange = false;

    [SerializeField]
    private bool stopOnAttack = false;
    
    public override int ChangeState_Enter(UnitAIController controller)
    {
        if (controller.AIStateP == AIState.Overwatch)
        {
            controller.OnMoveDestination();
        }
        else
        {
            controller.OnMove();

        }

        return 0;
    }

    public override int ChangeState_Exit(UnitAIController controller)
    {
        controller.MoveStop();
        return 0;

    }

    public override int UpdateBehaviour(UnitAIController controller)
    {
        return 0;
    }

    public override int PeriodUpdateBehaviour(UnitAIController controller)
    {
        if (overwatchWhenInRange)
        {
            UnitController unit = DetectUnit(controller,controller.MoveStopRange);
            if (unit)
            {
                    controller.SetTarget(unit);
                    controller.ChangeState(AIState.Attack);
                    return 1;
            }
        }
        if (!controller.IsMoving)
        {
            controller.ChangeState(AIState.Overwatch);
            return 1;
        }

        return 0;
    }

    public override void OnTakeDamage(UnitAIController controller, DamageData damageData, LifeSystem source)
    {
        if (stopOnAttack)
        {
            controller.MoveStop();
        }
    }
}
