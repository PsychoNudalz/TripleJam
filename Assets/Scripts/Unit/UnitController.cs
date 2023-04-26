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
    private Movement movement;

    [SerializeField]
    private Vector3 targetPosition;
    [SerializeField]
    Vector3 facingDirection;

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

    public bool IsFriendly(UnitController other)
    {
        return other.faction.Equals(faction);
    }
    
    public bool IsHostile(UnitController other)
    {
        return !other.faction.Equals(faction);
    }

    public void Init()
    {
        
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
        if(movement is WaypointMovement wm)
        {
            wm.Move_NewPoint_FromPos(newPoint);
        }
    }

    public void OnMove()
    {
        movement.MoveToTarget(targetPosition,facingDirection);
    }

    public void SetTargetPos(Vector3 pos, Vector3 dir)
    {
        targetPosition = pos;
        facingDirection = dir;

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
}
