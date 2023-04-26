using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    protected enum MoveStat
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
    protected float moveSpeed = 2f;


    [SerializeField]
    protected float deadZone = .1f;

    [SerializeField]
    protected float acceleration = .1f;

    protected float slowDownZone => (current_velocity.magnitude * current_velocity.magnitude) / (2 * acceleration);

    [SerializeField]
    protected float freezeResumeDelayTime = 1f;

    protected Coroutine freezeResumeDelayCoroutine;

    [Header("Rotate")]
    [SerializeField]
    protected float rotationSpeed = 2f;

    protected float rotateLerp = 0;

    [Header("Current")]
    [SerializeField]
    protected MoveStat moveState = MoveStat.Still;

    [SerializeField]
    protected Vector3 current_velocity;

    protected Vector3 target_pos;

    protected Vector3 original_rot;
    protected Vector3 target_rot;
    protected Vector3 target_forward;
    public bool IsMoving => moveState is MoveStat.Move or MoveStat.Rotate;

    protected Vector3 position
    {
        get => transform.position;
        set => transform.position = value;
    }

    protected float distance => Vector3.Distance(target_pos, position);

    protected Vector3 direction => (target_pos - position).normalized;

    // Start is called before the first frame update
    void Start()
    {
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


    protected virtual void UpdateBehaviour()
    {
    }
    protected virtual void Move_Translate()
    {
        if (distance < deadZone)
        {
            StopMove();
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

    public virtual void StopMove()
    {
        current_velocity = Vector3.zero;
        ChangeState(MoveStat.Still);
    }

    protected void SetPositionToTarget()
    {
        current_velocity = Vector3.zero;
        position = target_pos;
    }

    public virtual void MoveToTarget(Vector3 pos, Vector3 dir)
    {
        target_rot = Quaternion.LookRotation(dir).eulerAngles;

        Vector3 moveDir = (pos - transform.position).normalized;
        target_forward = Quaternion.LookRotation(moveDir).eulerAngles;

        target_pos = pos;
        ChangeState(MoveStat.Move);
    }
    
    

    protected virtual void ChangeState(MoveStat s)
    {
        switch (moveState)
        {
            case MoveStat.Still:
                rotateLerp = 0;
                break;
            case MoveStat.Move:
                rotateLerp = 0;
                original_rot = transform.eulerAngles;

                break;
            case MoveStat.Freeze:
                if (s != MoveStat.Unfreeze)
                {
                    return;
                }

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
                Move_Rotate(target_rot, true);
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

    public virtual void Move_Rotate(bool clockwise = true)
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

    public virtual void Move_Rotate(Vector3 target, bool forceLerp = false)
    {
        Vector3 transformEulerAngles = transform.eulerAngles;
        if (forceLerp)
        {
            rotateLerp = 2;
        }
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

    public virtual void SetFreeze(bool b)
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


    protected virtual IEnumerator DelayFreeze()
    {
        yield return new WaitForSeconds(freezeResumeDelayTime);
        ChangeState(MoveStat.Still);
    }

    public virtual void Move_Forward()
    {
        if (moveState != MoveStat.Still)
        {
            return;
        }
    }
}