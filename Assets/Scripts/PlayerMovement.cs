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
    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 2f;


    [SerializeField]
    private float deadZone = .02f;

    [SerializeField]
    private float acceleration = .1f;

    private float slowDownZone => (current_velocity.magnitude * current_velocity.magnitude) / (2 * acceleration);

    [Header("Rotate")]
    [SerializeField]
    private float rotationSpeed = 2f;

    private float rotateLerp = 0;

    [Header("Target")]
    [SerializeField]
    private WaypointController waypoint;

    [SerializeField]
    private Vector3 target_pos;

    [SerializeField]
    private Vector3 target_rot;
    
    
    [SerializeField]
    private Vector3 target_forward;

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

    private float distance => Vector3.Distance(target_pos, position);

    private Vector3 direction => (target_pos - position).normalized;
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
                Move_Rotate(target_rot);

                break;
            case MoveStat.Move:
                Move_Rotate(target_forward);

                Move_Translate();

                break;
            case MoveStat.Freeze:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Move_Translate()
    {
        if (distance < deadZone)
        {
            current_velocity = Vector3.zero;
            position = target_pos;
            ChangeState(MoveStat.Still);
        }
        else
        {
            if (distance < slowDownZone)
            {
                current_velocity -= direction * (acceleration * Time.deltaTime);
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
            }

            transform.Translate(current_velocity * Time.deltaTime, Space.World);
        }
    }

    void Move_Rotate(Vector3 target)
    {
        Vector3 transformEulerAngles = transform.eulerAngles;

        if (rotateLerp > 1)
        {
            rotateLerp = 1;
        }
        else if (rotateLerp < 1)
        {
            rotateLerp += Time.deltaTime * rotationSpeed;
        }
        transformEulerAngles.y = Mathf.LerpAngle(transformEulerAngles.y,target.y,rotateLerp);


        transform.eulerAngles = transformEulerAngles;
    }
   

    void ChangeState(MoveStat s)
    {
        switch (moveStat)
        {
            case MoveStat.Still:
                break;
            case MoveStat.Move:
                rotateLerp = 0;
                break;
            case MoveStat.Freeze:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        moveStat = s;
        switch (moveStat)
        {
            case MoveStat.Still:
                break;
            case MoveStat.Move:
                rotateLerp = 0;
                break;
            case MoveStat.Freeze:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Move_Forward()
    {
        if (moveStat != MoveStat.Still)
        {
            return;
        }
        
        WaypointController newPoint = waypoint.GetConnected(transform.forward);

        target_forward = waypoint.GetDir(newPoint);

        target_rot = newPoint.transform.eulerAngles;
        if (Vector3.Dot(target_forward, newPoint.forward) < 0)
        {
            target_rot.y -= 180;
        }

        target_forward = Quaternion.LookRotation(target_forward).eulerAngles;

        waypoint = newPoint;


        target_pos = waypoint.position;
        ChangeState(MoveStat.Move);
    }
}