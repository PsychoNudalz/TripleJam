using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public enum AIState
{
    Idle,
    Move,
    Overwatch,
    Attack,
    Reload,
    Dead
}

public class UnitAIController : MonoBehaviour
{
    [SerializeField]
    private UnitController unitController;

    [SerializeField]
    private AIState aiState = AIState.Idle;

    [SerializeField]
    private AIBehaviour currentBehaviour;

    [SerializeField]
    private UnitAIBehaviour_Set unitAIBehaviourSet;

    [Header("States")]
    [SerializeField]
    private bool isStationary = true;

    [Header("Settings")]
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private float attackRange;

    [Header("Attack")]
    [SerializeField]
    private UnitAttack attack;

    [SerializeField]
    private UnitController targetUnit;

    public LayerMask LayerMask => layerMask;

    public float AttackRange => attackRange;

    public bool IsStationary => isStationary;
    public Vector3 Position => unitController.Position;

    public UnitController UnitController => unitController;

    public UnitAttack Attack => attack;

    public float AttackCooldown => attack.Cooldown;

    private void Awake()
    {
        if (!unitController)
        {
            unitController = GetComponent<UnitController>();
        }

        if (!currentBehaviour)
        {
            currentBehaviour = unitAIBehaviourSet.IdleAIBehaviour;
        }

        unitAIBehaviourSet = Instantiate(unitAIBehaviourSet, transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(aiState);
    }

    // Update is called once per frame
    void Update()
    {
        currentBehaviour.UpdateBehaviour(this);
    }

    private void FixedUpdate()
    {
        currentBehaviour.FixedUpdateBehaviour(this);
    }

    public void ChangeState(AIState aiState)
    {
        currentBehaviour.ChangeState_Exit(this);
        Debug.Log($"{this} change state: {this.aiState} => {aiState}");
        currentBehaviour = unitAIBehaviourSet.GetBehaviour(aiState);
        this.aiState = aiState;
        currentBehaviour.ChangeState_Enter(this);
    }

    public void SetMovePosition(WaypointController newPoint)
    {
        unitController.OnMove(newPoint);
    }

    public void SetMovePosition()
    {
        unitController.OnMove();
    }

    public bool IsMoving => unitController.IsMoving;

    public void OnAttack()
    {
        if (attack)
        {
            attack.OnAttack_Enter(targetUnit);
        }
    }

    public void SetTarget(UnitController target)
    {
        targetUnit = target;
    }
}