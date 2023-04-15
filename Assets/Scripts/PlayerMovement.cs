using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    enum MoveStat
    {
        Still,
        Move,
        Freeze
    }

    [Header("Stats")]
    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private float deadZone = .2f;

    [SerializeField]
    private float acceleration = .1f;

    [SerializeField]
    private WaypointController waypoint;

    [SerializeField]
    private Vector3 target;

    [Header("Current")]
    [SerializeField]
    private MoveStat moveStat = MoveStat.Still;

    [SerializeField]
    private Vector3 current_velocity;

    [Header("Components")]
    [SerializeField]
    private CharacterController cc;

    [SerializeField]
    private WaypointManager waypointManager;

    private Vector3 position
    {
        get => transform.position;
        set => transform.position = value;
    }

    private float distance => Vector3.Distance(target, position);

    private Vector3 direction => (target - position).normalized;
    // Start is called before the first frame update

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (!waypointManager)
        {
            waypointManager = FindObjectOfType<WaypointManager>();
        }
    }

    void Start()
    {
        if (waypointManager)
        {
            position = waypointManager.StartingPosition;
            waypoint = waypointManager.StartingPoint;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (moveStat)
        {
            case MoveStat.Still:
                break;
            case MoveStat.Move:
                if (distance < deadZone)
                {
                    current_velocity = Vector3.zero;
                    position = target;
                    ChangeState(MoveStat.Still);
                }
                else
                {
                    if (current_velocity.magnitude < moveSpeed)
                    {
                        current_velocity += direction * (acceleration * Time.deltaTime);
                    }
                    else if (current_velocity.magnitude > moveSpeed)
                    {
                        current_velocity = direction * moveSpeed;
                    }

                    cc.Move(current_velocity * Time.deltaTime);
                }

                break;
            case MoveStat.Freeze:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void ChangeState(MoveStat s)
    {
        moveStat = s;
    }

    public void Move_Forward()
    {
        if (moveStat == MoveStat.Move)
        {
            return;
        }
        WaypointController newPoint = waypoint.GetConnected(transform.forward);
        if (Vector3.Dot(waypoint.GetDir(newPoint), waypoint.forward) < 0)
        {
            transform.forward = -waypoint.forward;
        }
        else
        {
            transform.forward = waypoint.forward;
        }

        waypoint = newPoint;
        
        
        target = waypoint.position;
        ChangeState(MoveStat.Move);

        
    }
}