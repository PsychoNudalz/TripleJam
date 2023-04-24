using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class AIBehaviour
{
    // return values:
    // 0: okay
    // 1: changed state
    // -1: error
    public abstract int ChangeState_Enter(UnitAIController controller);
    public abstract int ChangeState_Exit(UnitAIController controller);
    public abstract int Update(UnitAIController controller);
    public abstract int FixedUpdate(UnitAIController controller);

}
