using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Unit_AI/Behavior/Reload")]

public class Reload_AIBehaviour : AIBehaviour
{
    private float reloadTime = 0;

    [SerializeField]
    private bool retreatFromDamage = false;
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
        reloadTime -= Time.deltaTime;
        if (reloadTime < 0)
        {
            controller.ChangeState(AIState.Overwatch);
            return 1;
        }
        return 0;
    }

    public override int PeriodUpdateBehaviour(UnitAIController controller)
    {

        return 0;
    }

    public override void OnTakeDamage(UnitAIController controller, DamageData damageData, LifeSystem source)
    {
        if (retreatFromDamage)
        {
            controller.RetreatFromDamage(damageData);
            controller.ChangeState(AIState.Move);
        }
    }
}