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
    private float moveStopRange = 5f;
    [SerializeField]
    private float attackRange = 10f;

    [Header("Attack")]
    [SerializeField]
    private UnitAttack attack;

    private UnitController targetUnit;

    public LayerMask LayerMask => layerMask;


    public float MoveStopRange => moveStopRange;

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
        unitAIBehaviourSet.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(aiState);
    }

    // Update is called once per frame
    void Update()
    {
        if (aiState == AIState.Dead)
        {
            return;
        }
        currentBehaviour.UpdateBehaviour(this);
    }

    private void FixedUpdate()
    {
        if (aiState == AIState.Dead)
        {
            return;
        }
        currentBehaviour.FixedUpdateBehaviour(this);
    }

    public void ChangeState(AIState aiState)
    {
        currentBehaviour.ChangeState_Exit(this);
        // Debug.Log($"{this} change state: {this.aiState} => {aiState}");
        if (aiState == AIState.Dead)
        {
            enabled = false;
            return;
        }
        currentBehaviour = unitAIBehaviourSet.GetBehaviour(aiState);
        this.aiState = aiState;
        currentBehaviour.ChangeState_Enter(this);
    }

    public void SetMovePosition(WaypointController newPoint)
    {
        unitController.OnMove(newPoint);
    }
    
    public void SetMovePosition(Vector3 pos,Vector3 dir)
    {
        unitController.SetTargetPos(pos,dir);
    }


    public void OnMove()
    {
        unitController.OnMove();
    }

    public void MoveStop()
    {
        unitController.MoveStop();
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

    public void OnTakeDamage(DamageData damageData, LifeSystem source)
    {
        currentBehaviour.OnTakeDamage(this, damageData, source);
    }
    
    public void RetreatFromDamage(DamageData damageData)
    {
        unitController.SetTargetPos(unitController.GetRetreatPos(damageData));
    }
}