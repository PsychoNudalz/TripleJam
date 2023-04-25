using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInputController : MonoBehaviour
{
    [FormerlySerializedAs("playerMovement")]
    [Header("Components")]
    [SerializeField]
    WaypointMovement waypointMovement;

    [SerializeField]
    private WaypointController waypointController;

    [SerializeField]
    private OverviewCameraMovement overviewCameraMovement;

    [SerializeField]
    private UserCursorFromCameraController userCursorFromCameraController;
    

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
        print("MoveCam");
        float dir = inputValue.Get<float>();
            overviewCameraMovement.OnMove_Local((new Vector2(dir,0 )).normalized);
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
}