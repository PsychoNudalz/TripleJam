using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using Range = UnityEngine.SocialPlatforms.Range;

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
    private float buttonSpawnTime = 1;
    private float buttonSpawnTime_now = 0;

    [SerializeField]
    private float captureDelay = 0.5f;
    [Header("Button Settings")]
    [SerializeField]
    private Transform centrePoint;

    [SerializeField]
    private SpriteRenderer button;

    [SerializeField]
    private Sprite[] buttonSprites;

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
        button.gameObject.SetActive(false);
    }

    private void Update()
    {
        switch (workState)
        {
            case ComputerWorkState.Input:
                break;
            case ComputerWorkState.Wait:
                if (buttonSpawnTime_now < 0)
                {
                    ChangeState(ComputerWorkState.Ready);
                }
                else
                {
                    buttonSpawnTime_now -= Time.deltaTime;
                }
                break;
            case ComputerWorkState.Ready:
                DisplayRandomButton();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void ChangeState(ComputerWorkState state)
    {
        switch (workState)
        {
            case ComputerWorkState.Input:
                button.gameObject.SetActive(false);
                break;
            case ComputerWorkState.Wait:
                break;
            case ComputerWorkState.Ready:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        workState = state;
        switch (state)
        {
            case ComputerWorkState.Input:
                break;
            case ComputerWorkState.Wait:
                buttonSpawnTime_now = buttonSpawnTime;
                break;
            case ComputerWorkState.Ready:
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void OnMove(InputValue inputValue)
    {
        if (workState == ComputerWorkState.Input)
        {
            if (inputValue.Get<Vector2>().magnitude > .1f)
            {
                ChangeState(ComputerWorkState.Wait);
            }
        }
    }

    public void On_WorkComputer_Enter()
    {
        vCam.Priority = 20;
        playerMovement.SetFreeze(true);
        playerInput.enabled = true;
        // playerHeadMaker.FaceInit();
        ChangeState(ComputerWorkState.Ready);
    }

    public void On_WorkComputer_Exit()
    {
        vCam.Priority = 0;
        playerMovement.SetFreeze(false);
        playerInput.enabled = true;
    }

    public void DisplayRandomButton()
    {
        button.gameObject.SetActive(true);

        button.sprite = buttonSprites[Random.Range(0, buttonSprites.Length)];
        Vector3 pos =centrePoint.localPosition;
        float range = normalRange;
        if (presses_Current % captureMod==0)
        {
            range = cameraRange;
            StartCoroutine(DelayCapture(captureDelay));
        }

        if (clampY)
        {
            pos += new Vector3(Random.Range(-range, range), Random.Range(-range, 0));
        }
        else
        {
            pos += new Vector3(Random.Range(-range, range), Random.Range(-range, range));
        }

        button.transform.localPosition = pos;
        presses_Current++;

        ChangeState(ComputerWorkState.Input);

    }

    IEnumerator DelayCapture(float t)
    {
        yield return new WaitForSeconds(t);
        playerHeadMaker.OnCapturePlayerHead();
    }
}