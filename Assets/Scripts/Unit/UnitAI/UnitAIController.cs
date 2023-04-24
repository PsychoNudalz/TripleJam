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
    private AIBehaviour currentBehaviour = new Idle_AIBehaviour();

    [Header("States")]
    [SerializeField]
    private bool isStationary = true;

    [Header("Settings")]
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private float attackRange;

    public LayerMask LayerMask => layerMask;

    public float AttackRange => attackRange;

    public bool IsStationary => isStationary;
    public Vector3 Position => unitController.Position;

    public UnitController UnitController => unitController;

    private void Awake()
    {
        if (!unitController)
        {
            unitController = GetComponent<UnitController>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(aiState);
    }

    // Update is called once per frame
    void Update()
    {
        currentBehaviour.Update(this);
    }

    private void FixedUpdate()
    {
        currentBehaviour.FixedUpdate(this);
    }

    public void ChangeState(AIState aiState)
    {
        currentBehaviour.ChangeState_Exit(this);
        Debug.Log($"{this} change state: {this.aiState} => {aiState}");
        
        switch (aiState)
        {
            case AIState.Idle:
                currentBehaviour = new Idle_AIBehaviour();
                break;
            case AIState.Move:
                currentBehaviour = new Move_AIBehaviour();

                break;
            case AIState.Overwatch:
                currentBehaviour = new Overwatch_AIBehaviour();
                break;
            case AIState.Attack:
                
                break;
            case AIState.Reload:
                
                break;
            case AIState.Dead:
                break;
        }

        this.aiState = aiState;
        currentBehaviour.ChangeState_Enter(this);
    }

    public void SetMovePosition(WaypointController newPoint = null)
    {
        unitController.OnMove(newPoint);
    }

    public bool IsMoving => unitController.IsMoving;
}
