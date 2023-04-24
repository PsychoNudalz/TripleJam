using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Unit_AI/Behavior/Base")]
public abstract class AIBehaviour: ScriptableObject
{
    // return values:
    // 0: okay
    // 1: changed state
    // -1: error
    public abstract int ChangeState_Enter(UnitAIController controller);

    public abstract int ChangeState_Exit(UnitAIController controller);

    public abstract int UpdateBehaviour(UnitAIController controller);

    public abstract int FixedUpdateBehaviour(UnitAIController controller);
}
