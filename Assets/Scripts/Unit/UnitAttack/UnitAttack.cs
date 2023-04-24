using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class UnitAttack : MonoBehaviour
{
    enum AttackState
    {
        Start,
        Action,
        End
    }

    private AttackState attackState = AttackState.Start;
    [SerializeField]
    protected UnityEvent onAttackEvent;

    [SerializeField]
    protected float damage = 10f;

    [SerializeField]
    protected float cooldown = 5f;
    
    [SerializeField]
    protected LayerMask layerMask;

    [FormerlySerializedAs("seperateAction")]
    [SerializeField]
    protected bool separateAction = false;

    public bool IsAttacking => !(attackState is AttackState.End);

    public float Cooldown => cooldown;

    public virtual void OnAttack_Enter(UnitController target)
    {
        onAttackEvent.Invoke();
        if (!separateAction)
        {
            OnAttack_Action(target);
            OnAttack_Exit(target);
        }
    }

    public virtual void OnAttack_Action(UnitController target)
    {
        attackState = AttackState.Action;
        Debug.Log($"{this} attack => {target}");
    }

    public virtual void OnAttack_Exit(UnitController target)
    {
        attackState = AttackState.End;

    }
}
