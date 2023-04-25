using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMovement : Movement
{


    [Header("Stats")]



    [Header("Target")]
    [SerializeField]
    private WaypointController waypoint;





    [Header("Components")]

    [SerializeField]
    private WaypointManager waypointManager;


    // Start is called before the first frame update

    private void Awake()
    {
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
        UpdateBehaviour();
    }

    protected override void ChangeState(MoveStat s)
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
                SetPositionToTarget();

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
                Move_Rotate(target_rot,true);
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

    public override void Move_Forward()
    {
        if (moveState != MoveStat.Still)
        {
            return;
        }

        WaypointController newPoint = waypoint.GetConnected(transform.forward);

        target_forward = waypoint.GetDir(newPoint);

        MoveToPoint(newPoint);
    }

    private void MoveToPoint(WaypointController newPoint)
    {
        target_rot = newPoint.transform.eulerAngles;
        if (!newPoint.HardRotate)
        {
            if (Vector3.Dot(target_forward, newPoint.forward) < 0)
            {
                target_rot.y -= 180;
            }
        }

        target_forward = Quaternion.LookRotation(target_forward).eulerAngles;

        waypoint = newPoint;


        target_pos = waypoint.position;
        ChangeState(MoveStat.Move);
    }

    public void Move_NewPoint_FromPos(WaypointController newPoint)
    {
        target_forward = (newPoint.position - transform.position);

        MoveToPoint(newPoint);

    }

    public override void Move_Rotate(bool clockwise = true)
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

}