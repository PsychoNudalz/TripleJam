using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// this is for handling the cursor controls
/// </summary>
public class UserCursorFromCameraController : MonoBehaviour
{
    [Header("Cursor Control")]
    Mouse mouse;
    [SerializeField] GameObject cursor;
    [SerializeField] float yOffset = .2f;
    [SerializeField] bool isMouse = true;
    [SerializeField] bool isController = false;
    [SerializeField] Camera cameraObject;
    [SerializeField] LayerMask layerMask;

    [Header("Optimisation")]
    [SerializeField] Vector3 targetPos;
    [SerializeField] float refereshRate;
    [SerializeField] float timeNow_refereshRate;




    public GameObject Cursor { get => cursor; set => cursor = value; }

    private void Awake()
    {
        if (cameraObject == null)
        {
            cameraObject = FindObjectOfType<Camera>();
        }

        //mouse = GetComponent<Mouse>();
    }

    private void FixedUpdate()
    {
        if (isMouse && Time.time - timeNow_refereshRate > refereshRate)
        {
            timeNow_refereshRate = Time.deltaTime;
            CastUpdateWithMouse();
        }

        UpdateCursor();
    }

    public void MoveWithMouse(InputAction.CallbackContext inputAction)
    {
        isMouse = true;
        isController = false;
    }

    public void MoveWithAxis(InputAction.CallbackContext inputAction)
    {
        isMouse = false;
        isController = true;
    }
    
    public void OnMoveWithMouse(InputValue inputValue)
    {
        isMouse = true;
        isController = false;
    }

    public void OnMoveWithAxis(InputValue inputValue)
    {
        isMouse = false;
        isController = true;
    }

    /// <summary>
    /// move player token to the tile the cursor is on
    /// </summary>
    /// <param name="inputAction"></param>
    public void MovePlayerToken(InputAction.CallbackContext inputAction)
    {

    }
    /// <summary>
    /// raycast from the camera to board to update the cursor's target position
    /// </summary>
    void CastUpdateWithMouse()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraObject.transform.position, cameraObject.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, yOffset)) - cameraObject.transform.position, out hit, 500f, layerMask))
        {
            //print(Mouse.current.position.ReadValue());
            targetPos = hit.point + new Vector3(0, yOffset);

        }
        Debug.DrawRay(cameraObject.transform.position, (cameraObject.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, yOffset)) - cameraObject.transform.position) * 500f, Color.green);

    }
    /// <summary>
    /// update lerp transform the cursor's location to the target position
    /// </summary>
    void UpdateCursor()
    {
        if (!cursor)
        {
            Debug.LogError("SDFGHJK");
        }
        cursor.transform.position = Vector3.Lerp(cursor.transform.position, targetPos, 40 * Time.deltaTime);
    }

}