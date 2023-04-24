using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unit_AI/Set")]
public class UnitAIBehaviour_Set : ScriptableObject
{

    [SerializeField]
    private Idle_AIBehaviour idleAIBehaviour;

    [SerializeField]
    private Move_AIBehaviour moveAIBehaviour;

    [SerializeField]
    private Overwatch_AIBehaviour overwatchAIBehaviour;

    [SerializeField]
    private Attack_AIBehaviour attackAIBehaviour;

    [SerializeField]
    private Reload_AIBehaviour reloadAIBehaviour;

    public Idle_AIBehaviour IdleAIBehaviour => idleAIBehaviour;

    public Move_AIBehaviour MoveAIBehaviour => moveAIBehaviour;

    public Overwatch_AIBehaviour OverwatchAIBehaviour => overwatchAIBehaviour;

    public Attack_AIBehaviour AttackAIBehaviour => attackAIBehaviour;

    public Reload_AIBehaviour ReloadAIBehaviour => reloadAIBehaviour;

    private void Awake()
    {
        idleAIBehaviour = Instantiate(idleAIBehaviour);
        moveAIBehaviour = Instantiate(moveAIBehaviour);
        overwatchAIBehaviour = Instantiate(overwatchAIBehaviour);
        attackAIBehaviour = Instantiate(attackAIBehaviour);
        reloadAIBehaviour = Instantiate(reloadAIBehaviour);
    }

    public AIBehaviour GetBehaviour(AIState aiState)
    {
        switch (aiState)
        {
            case AIState.Idle:
                return idleAIBehaviour;
                break;
            case AIState.Move:
                return moveAIBehaviour;
                break;
            case AIState.Overwatch:
                return overwatchAIBehaviour;
                break;
            case AIState.Attack:
                return attackAIBehaviour;
                break;
            case AIState.Reload:
                return reloadAIBehaviour;
                break;
            case AIState.Dead:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(aiState), aiState, null);
        }

        return idleAIBehaviour;
    }
}