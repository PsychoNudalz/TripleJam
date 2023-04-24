using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class OverviewCameraMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private Vector2 moveDir;

    private void Update()
    {
        if (moveDir.magnitude > .2f)
        {
            transform.Translate(moveDir*moveSpeed);
        }
    }

    public void OnMove_Local(Vector2 newMoveDir)
    {
        moveDir = newMoveDir;
    }
}