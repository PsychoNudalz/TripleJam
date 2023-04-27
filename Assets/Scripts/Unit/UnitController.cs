using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class UnitController : MonoBehaviour
{
    [SerializeField]
    private UnitAIController aiController;

    [SerializeField]
    private UnitLifeSystem unitLifeSystem;

    [SerializeField]
    private Movement movement;

    [FormerlySerializedAs("targetPosition")]
    [SerializeField]
    private Vector3 MovePosition;


    [SerializeField]
    Vector3 facingDirection;

    [SerializeField]
    private Vector3 destinationPosition;

    [Header("States")]
    [SerializeField]
    private UnitFaction faction = UnitFaction.Defender;

    [SerializeField]
    private int cost = 0;

    [FormerlySerializedAs("OnSpawn")]
    [Header("Events")]
    [SerializeField]
    private UnityEvent onSpawnEvent;

    [SerializeField]
    private UnityEvent onDeathEvent;

    public Vector3 Position => transform.position;
    public bool IsMoving => movement.IsMoving;

    public int Cost => cost;
    public LifeSystem ls => unitLifeSystem;

    public Vector3 AttackDir => facingDirection;

    public bool IsFriendly(UnitController other)
    {
        return other.faction.Equals(faction);
    }

    public bool IsHostile(UnitController other)
    {
        return !other.faction.Equals(faction);
    }

    public void Init(Vector3 pos, Vector3 dir = default)
    {
        destinationPosition = pos;
        SetMovePos(pos,dir);
    }

    private void Awake()
    {
        if (unitLifeSystem)
        {
            unitLifeSystem = GetComponent<UnitLifeSystem>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        onSpawnEvent.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnMove(WaypointController newPoint)
    {
        if (!newPoint)
        {
            newPoint = WaypointController.main;
        }

        if (movement is WaypointMovement wm)
        {
            wm.Move_NewPoint_FromPos(newPoint);
        }
    }

    public void OnMove()
    {
        movement.MoveToTarget(MovePosition, facingDirection);
    }

    public void SetTargetPos(Vector3 pos, Vector3 dir = default)
    {
        destinationPosition = pos;
        MovePosition = pos;
        if (!dir.Equals(default))
        {
            facingDirection = dir;
        }
    }
    
    public void SetMovePos(Vector3 pos, Vector3 dir = default)
    {
        MovePosition = pos;
        if (!dir.Equals(default))
        {
            facingDirection = dir;
        }
    }



    public Vector3 GetRetreatPos(DamageData damageData)
    {
        float distance = damageData.Range - (transform.position - damageData.Point).magnitude;
        distance += damageData.Range *.5f;
        Vector3 pos = transform.position + distance* -AttackDir;
        return pos;
    }

    public void OnDeath()
    {
        onDeathEvent.Invoke();
        MoveStop();
        Destroy(gameObject, 2f);
    }

    public void MoveStop()
    {
        movement.StopMove();
    }
    

    public void OnTakeDamage(LifeSystem source, DamageData damageData)
    {
        // if (!source)
                    // {
                    // }
        aiController.OnTakeDamage(damageData, source);
    }

    public void OnTakeDamage(LifeSystem source)
    {
        if (!source)
        {
            return;
        }
    }
}