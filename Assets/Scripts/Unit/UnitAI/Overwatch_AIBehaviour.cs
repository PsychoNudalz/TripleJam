using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Unit_AI/Behavior/Overwatch")]

public class Overwatch_AIBehaviour : AIBehaviour
{
    [SerializeField]
    private bool retreatFromDamage;

    [SerializeField]
    private bool notAttack;

    [SerializeField]
    private bool moveIfNoUnitsFound;

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

    public override int PeriodUpdateBehaviour(UnitAIController controller)
    {
        if (notAttack)
        {
            return 0;
        }

        int unitCheck = CheckForUnit(controller);
        if (moveIfNoUnitsFound && unitCheck == 0)
        {
            controller.ChangeState(AIState.Move);
            return 2;
        }
        return unitCheck;
    }

    public override void OnTakeDamage(UnitAIController controller, DamageData damageData, LifeSystem source)
    {
        if (retreatFromDamage)
        {
            controller.RetreatFromDamage(damageData);
            controller.ChangeState(AIState.Move);

        }
    }

    private int CheckForUnit(UnitAIController controller)
    {
        UnitController foundUnit = DetectUnit(controller,controller.AttackRange);
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
