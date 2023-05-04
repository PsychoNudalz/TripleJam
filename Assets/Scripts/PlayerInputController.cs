using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField]
    private bool isLock = false;

    [SerializeField]
    private GameObject lockDisplay;

    [Header("MouseToWorld")]
    [SerializeField]
    private float mouseToWorldRange = 10;

    [SerializeField]
    private float minMouseMoveDistance = 2f;


    [Header("Components")]
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    WaypointMovement waypointMovement;

    [SerializeField]
    private WaypointController waypointController;

    [SerializeField]
    private OverviewCameraMovement overviewCameraMovement;

    [SerializeField]
    private UserCursorFromCameraController userCursorFromCameraController;

    [SerializeField]
    private PlayerSliceController playerSliceController;

    [Header("Debug")]
    [SerializeField]
    private Vector3 screenToWorldPoint;

    [SerializeField]
    private Vector2 mouseAngle;


    [SerializeField]
    private Vector2 mouseMoveDir;

    [SerializeField]
    private Vector2 lastMousePosition;

    private float mouseMoveDir_Angle;

    public static PlayerInputController current;


    // Start is called before the first frame update
    void Awake()
    {
        if (!waypointMovement)
        {
            waypointMovement = GetComponent<WaypointMovement>();
        }

        if (!overviewCameraMovement)
        {
            overviewCameraMovement = GetComponent<OverviewCameraMovement>();
        }

        if (!userCursorFromCameraController)
        {
            userCursorFromCameraController = GetComponent<UserCursorFromCameraController>();
        }

        if (!mainCamera)
        {
            mainCamera = Camera.main;
        }

        lastMousePosition = transform.position;

        current = this;
    }

    private void Start()
    {
        OnLock(isLock);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnMove(InputValue inputValue)
    {
        if (isLock)
        {
            return;
        }

        Vector2 dir = inputValue.Get<Vector2>();
        if (waypointMovement)
        {
            if (dir.y > 0.5)
            {
                waypointMovement.Move_Forward();
            }

            if (Mathf.Abs(dir.x) > .1f)
            {
                waypointMovement.Move_Rotate(dir.x > 0);
            }
        }
    }

    public void OnMoveCamera(InputValue inputValue)
    {
        if (isLock)
        {
            return;
        }

        // print("MoveCam");
        float dir = inputValue.Get<float>();
        overviewCameraMovement.OnMove_Local((new Vector2(dir, 0)).normalized);
    }

    public void OnClick(InputValue inputValue)
    {
        if (isLock)
        {
            return;
        }

        if (inputValue.isPressed)
        {
            UpdateWaypointToCursor();
        }
    }

    public void UpdateWaypointToCursor()
    {
        if (isLock)
        {
            return;
        }

        if (waypointController)
        {
            waypointController.transform.position = userCursorFromCameraController.target;
        }
    }

    public void OnMouseMove(InputValue inputValue)
    {
        if (isLock)
        {
            return;
        }


        Vector2 mousePosition = inputValue.Get<Vector2>();

        // mouse to world
        Vector3 mouse3D = new Vector3(mousePosition.x, mousePosition.y, mouseToWorldRange);
        screenToWorldPoint = mainCamera.ScreenToWorldPoint(mouse3D);
        mouseAngle.x = -Mathf.Atan(mouse3D.y / mouse3D.z) * Mathf.Rad2Deg;
        mouseAngle.y = Mathf.Atan(mouse3D.x / mouse3D.z) * Mathf.Rad2Deg;

        //Checking mouse move dir
        Vector2 dis = mousePosition - lastMousePosition;
        if (dis.magnitude > minMouseMoveDistance)
        {
            mouseMoveDir = dis.normalized;
            mouseMoveDir_Angle = Mathf.Atan2(mouseMoveDir.x, mouseMoveDir.y) * Mathf.Rad2Deg;
            lastMousePosition = mousePosition;
        }

        playerSliceController.UpdateController(screenToWorldPoint, mouseMoveDir_Angle);
    }

    public void OnSwordDraw(InputValue inputValue)
    {
        if (isLock)
        {
            return;
        }

        if (inputValue.isPressed)
        {
            playerSliceController.SetDraw(true);
        }
        else
        {
            playerSliceController.SetDraw(false);
        }
    }

    public static void SetLock(bool b)
    {
        current.OnLock(b);
    }

    public void OnLock(bool b)
    {
        if (b)
        {
            playerSliceController?.SetDraw(false);
        }

        isLock = b;
        lockDisplay.SetActive(isLock);
    }
}