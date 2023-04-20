using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComputerWork : MonoBehaviour
{
    enum ComputerWorkState
    {
        Input,
        Wait,
        Ready
    }

    [Header("State")]
    [SerializeField]
    private ComputerWorkState workState = ComputerWorkState.Wait;

    [Header("Components")]
    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private CinemachineVirtualCamera vCam;

    [SerializeField]
    private PlayerHeadMaker playerHeadMaker;

    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField]
    private GameObject spook;

    // Start is called before the first frame update
    [Header("Computer")]
    [SerializeField]
    private Transform centrePoint;

    [SerializeField]
    private GameObject button;

    [SerializeField]
    private Texture2D[] buttonSprites;

    [SerializeField]
    private int captureMod = 2;

    [SerializeField]
    private int presses_Total = 16;

    private int presses_Current = 0;

    [SerializeField]
    private float normalRange = 1;

    [SerializeField]
    private float cameraRange = .2f;

    [SerializeField]
    private bool clampY = true;

    [SerializeField]
    void Start()
    {
        playerInput.enabled = false;
    }

    private void Update()
    {
    }

    public void OnMove(InputValue inputValue)
    {
    }

    public void On_WorkComputer_Enter()
    {
        vCam.Priority = 20;
        playerMovement.SetFreeze(true);
        playerInput.enabled = true;
        playerHeadMaker.FaceInit();
    }

    public void On_WorkComputer_Exit()
    {
        vCam.Priority = 0;
        playerMovement.SetFreeze(false);
        playerInput.enabled = true;
    }

    public void DisplayRandomButton()
    {
        
    }
}