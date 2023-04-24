using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UnitAttack : MonoBehaviour
{
    [SerializeField]
    protected UnityEvent onAttackEvent;

    [SerializeField]
    private float damage = 10f;
    public abstract void OnAttack(UnitController target);
}
