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
        Freeze,
        Rotate,
        Unfreeze
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

    [SerializeField]
    private float freezeResumeDelayTime = 1f;

    private Coroutine freezeResumeDelayCoroutine;
    
    [Header("Rotate")]
    [SerializeField]
    private float rotationSpeed = 2f;

    private float rotateLerp = 0;

    [Header("Target")]
    [SerializeField]
    private WaypointController waypoint;


    private Vector3 target_pos;

    private Vector3 original_rot;
    private Vector3 target_rot;
    private Vector3 target_forward;

    [Header("Current")]
    [SerializeField]
    private MoveStat moveState = MoveStat.Still;

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
        switch (moveState)
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
            case MoveStat.Rotate:
                Move_Rotate(target_rot);

                break;
            case MoveStat.Unfreeze:
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Move_Translate()
    {
        if (distance < deadZone)
        {
            SetPositionToWayPoint();
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

    private void SetPositionToWayPoint()
    {
        current_velocity = Vector3.zero;
        position = target_pos;
    }

    void Move_Rotate(Vector3 target)
    {
        Vector3 transformEulerAngles = transform.eulerAngles;

        if (rotateLerp > 1)
        {
            rotateLerp = 1;
            if (moveState == MoveStat.Rotate)
            {
                ChangeState(MoveStat.Still);
            }
        }
        else if (rotateLerp < 1)
        {
            rotateLerp += Time.deltaTime * rotationSpeed;
        }

        transformEulerAngles.y = Mathf.LerpAngle(original_rot.y, target.y, rotateLerp);


        transform.eulerAngles = transformEulerAngles;
    }


    void ChangeState(MoveStat s)
    {
        switch (moveState)
        {
            case MoveStat.Still:
                rotateLerp = 0;
                break;
            case MoveStat.Move:
                rotateLerp = 0;
                original_rot = transform.eulerAngles;
                if (s is MoveStat.Still or MoveStat.Rotate)
                {
                    waypoint.OnMove();
                    if (moveState == MoveStat.Freeze)
                    {
                        return;
                    }
                }
                break;
            case MoveStat.Freeze:
                if (s != MoveStat.Unfreeze)
                {
                    return;
                }
                SetPositionToWayPoint();

                break;
            case MoveStat.Rotate:
                original_rot = transform.eulerAngles;

                rotateLerp = 0;
                break;
            case MoveStat.Unfreeze:
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        moveState = s;
        switch (s)
        {
            case MoveStat.Still:
                rotateLerp = 0;
                break;
            case MoveStat.Move:
                original_rot = transform.eulerAngles;

                rotateLerp = 0;
                break;
            case MoveStat.Freeze:
                break;
            case MoveStat.Rotate:
                original_rot = transform.eulerAngles;

                rotateLerp = 0;
                break;
            case MoveStat.Unfreeze:
                StartCoroutine(DelayFreeze());

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Move_Forward()
    {
        if (moveState != MoveStat.Still)
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

    public void Move_Rotate(bool clockwise = true)
    {
        if (clockwise)
        {
            target_rot.y += 90f;
        }
        else
        {
            target_rot.y -= 90f;
        }

        ChangeState(MoveStat.Rotate);
    }

    public void SetFreeze(bool b)
    {
        if (b)
        {
            ChangeState(MoveStat.Freeze);
        }
        else
        {
            ChangeState(MoveStat.Unfreeze);
        }
    }


    IEnumerator DelayFreeze()
    {
        yield return new WaitForSeconds(freezeResumeDelayTime);
        ChangeState(MoveStat.Still);
    }
}