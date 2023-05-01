using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInputController : MonoBehaviour
{
    [Header("MouseToWorld")]
    [SerializeField]
    private float mouseToWorldRange = 10;

    [SerializeField]
    private float minMouseMoveDistance = 2f;

    [SerializeField]
    private Transform mainRotationalObject;

    [FormerlySerializedAs("mouseTipPosition")]
    [SerializeField]
    private Transform mouseTip;

    [SerializeField]
    private Transform visualRotationalObject;

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

    [Header("Debug")]
    [SerializeField]
    private Vector3 screenToWorldPoint;

    [SerializeField]
    private Vector2 mouseAngle;

    [SerializeField]
    private Vector2 mouseAngle_Offset;

    [SerializeField]
    private Vector2 mouseMoveDir;

    [SerializeField]
    private Vector2 lastMousePosition;

    private float mouseMoveDir_Angle;


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
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnMove(InputValue inputValue)
    {
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
        // print("MoveCam");
        float dir = inputValue.Get<float>();
        overviewCameraMovement.OnMove_Local((new Vector2(dir, 0)).normalized);
    }

    public void OnClick(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            UpdateWaypointToCursor();
        }
    }

    public void UpdateWaypointToCursor()
    {
        if (waypointController)
        {
            waypointController.transform.position = userCursorFromCameraController.target;
        }
    }

    public void OnMouseMove(InputValue inputValue)
    {
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
            mouseMoveDir_Angle = Mathf.Atan2(mouseMoveDir.x , mouseMoveDir.y) * Mathf.Rad2Deg;
            lastMousePosition = mousePosition;
        }

        UpdateMouseTip(screenToWorldPoint,mouseMoveDir_Angle);


        //Update object
        if (mainRotationalObject)
        {
            UpdateRotationObject(mainRotationalObject);
        }

        if (visualRotationalObject)
        {
            UpdateRotationObject(visualRotationalObject);
        }
    }

    void UpdateRotationObject(Transform t)
    {
        Vector3 dir = screenToWorldPoint - t.position;
        mouseAngle_Offset.x = -Mathf.Atan(dir.y / dir.z) * Mathf.Rad2Deg;
        mouseAngle_Offset.y = Mathf.Atan(dir.x / dir.z) * Mathf.Rad2Deg;
        Vector3 transformEulerAngles = t.eulerAngles;
        transformEulerAngles.x = mouseAngle_Offset.x;
        transformEulerAngles.y = mouseAngle_Offset.y;
        transformEulerAngles.z = mouseMoveDir_Angle;

        t.eulerAngles = transformEulerAngles;
    }

    void UpdateMouseTip(Vector3 pos, float rotationz)
    {
        if (!mouseTip)
        {
            return;
        }
        mouseTip.position = pos;
        Vector3 transformEulerAngles = mouseTip.eulerAngles;
        transformEulerAngles.z = rotationz;

        mouseTip.eulerAngles = transformEulerAngles;
    }
}