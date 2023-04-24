using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class UnitAttack : MonoBehaviour
{
    [SerializeField]
    protected UnityEvent onAttackEvent;

    [SerializeField]
    private float damage = 10f;

    [FormerlySerializedAs("seperateAction")]
    [SerializeField]
    private bool separateAction = false;

    public virtual void OnAttack_Enter(UnitController target)
    {
        onAttackEvent.Invoke();
        if (!separateAction)
        {
            OnAttack_Action(target);
        }
    }

    public virtual void OnAttack_Action(UnitController target)
    {
        
    }
}
