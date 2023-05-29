using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


public enum FlowScene
{
    None,
    Dojo_Opening,
    Dojo_StartSlice,
    Dojo_SpawnHarderCarrot,
    Dojo_FirstUpgrade,
    Dojo_SlicedBamboo,
    Dojo_Launchers,
    Dojo_Crowbar,
    Studio_Start,
    Studio_LightSaber,
    Studio_Couch,
    Studio_Collapse,
    Studio_Beg,
    Studio_DoIt,
    Studio_DoubleSaber,
    Studio_End,
    Bsod_Start,
    Bsod_Display,
    Bsod_Narrate,
    Bsod_AltF4,
    Bsod_End,
    MB_Start,
    MB_BugRemoval,
    MB_Ram,
    MB_GPU,
    MB_SSD,
    MB_Heat
}

public class GameFlowManager : MonoBehaviour
{
    [SerializeField]
    private FlowScene currentScene = FlowScene.None;

    [SerializeField]
    private GameObject player;

    [Header("Dojo")]
    [SerializeField]
    private GameObject titleScene;

    [SerializeField]
    private LauncherScript[] dojoLaunchers;

    [SerializeField]
    LauncherScript goldenLauncher;

    [SerializeField]
    private float openingTime = 5f;

    [SerializeField]
    private float sliceStartTime = 5f;

    [SerializeField]
    private float launcherSpeakTime = 5f;

    [SerializeField]
    private GameObject sliceableLaunchers;

    [SerializeField]
    private int goldenCarrotScore = 10;

    [SerializeField]
    private int bambooScore = 10;

    [SerializeField]
    private Camera preyCamera;

    [Header("Studio")]
    [SerializeField]
    private GameObject studio;

    [SerializeField]
    private Animator studioAnimator;

    [SerializeField]
    private Transform studioTeleportPoint;
    
    [SerializeField]
    private GameObject fallingSpotLight;

    [SerializeField]
    private int narratorRoomPillarCount = 4;

    [FormerlySerializedAs("BSOD Scene")]
    [SerializeField]
    private GameObject copyRightScreen;

    [SerializeField]
    private Sound elevatorMusic;
    [SerializeField]
    private Transform bsodTeleportPoint;

    [SerializeField]
    private float copyRightTime = 10f;

    [SerializeField]
    private SoundAbstract errorSound;
    [SerializeField]
    private float errorTime = 2;
    [SerializeField]
    private float errorToNarrateTime = 10f;

    [SerializeField]
    private Animator bsodAnimator;

    [SerializeField]
    private GameObject offButton;
    
    
    [Header("Motherboard")]
    [SerializeField]
    Transform MBTeleportPoint;

    [SerializeField]
    private GameObject motherboardFilter;
    [Header("Components")]

    [SerializeField]
    private NarratorManager narrator;

    public static GameFlowManager current;
    private Coroutine delaySceneTransition;
    private Coroutine waitScoreSceneTransition;


    private void Awake()
    {
        current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayStartOpening());
        preyCamera.gameObject.SetActive(false);
        sliceableLaunchers.SetActive(false);
        fallingSpotLight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void LateUpdate()
    {
    }


    public void Play_Scene(FlowScene flowScene, bool force = false)
    {
        if (flowScene <= currentScene && !force)
        {
            return;
        }

        Debug.Log($"Playing {flowScene}");
        switch (flowScene)
        {
            case FlowScene.None:
                break;
            case FlowScene.Dojo_Opening:
                Play_Dojo_Open();
                break;
            case FlowScene.Dojo_StartSlice:
                Play_Dojo_StartSlice();
                break;
            case FlowScene.Dojo_SpawnHarderCarrot:
                Play_Dojo_SpawnHarderCarrot();
                break;
            case FlowScene.Dojo_FirstUpgrade:
                Play_Dojo_FirstUpgrade();
                break;
            case FlowScene.Dojo_SlicedBamboo:
                Play_Dojo_SlicedBamboo();
                break;
            case FlowScene.Dojo_Launchers:
                Play_Dojo_Launchers();
                break;
            case FlowScene.Dojo_Crowbar:
                Play_Dojo_Crowbar();
                break;
            case FlowScene.Studio_Start:
                Play_Studio_Start();
                break;
            case FlowScene.Studio_LightSaber:
                Play_Studio_LightSaber();
                break;
            case FlowScene.Studio_Couch:
                Play_Studio_Couch();
                break;
            case FlowScene.Studio_Collapse:
                Play_Studio_Collapse();
                break;
            case FlowScene.Studio_Beg:
                Play_Studio_Beg();
                break;
            case FlowScene.Studio_DoIt:
                Play_Studio_DoIt();
                break;
            case FlowScene.Studio_DoubleSaber:
                Play_Studio_DoubleSaber();
                break;
            case FlowScene.Studio_End:
                Play_Studio_End();
                break;
            case FlowScene.Bsod_Start:
                Play_Bsod_Start();
                break;
            case FlowScene.Bsod_Display:
                Play_Bsod_Display();
                break;
            case FlowScene.Bsod_Narrate:
                Play_Bsod_Narrate();
                break;
            case FlowScene.Bsod_End:
                Play_Bsod_End();
                break;
            case FlowScene.MB_Start:
                Play_MB_Start();
                break;
            case FlowScene.MB_BugRemoval:
                Play_MB_BugRemoval();
                break;
            case FlowScene.MB_Ram:
                break;
            case FlowScene.MB_GPU:
                break;
            case FlowScene.MB_SSD:
                break;
            case FlowScene.MB_Heat:
                break;
            default:
                Debug.LogError($"Missing Scene: {flowScene}");
                break;
            
        }

        currentScene = flowScene;
    }

    public static void Play(FlowScene flowScene)
    {
        current.Play_Scene(flowScene);
    }

    void Play_Dojo_Open()
    {
        narrator.PlayAudio(FlowScene.Dojo_Opening);
        titleScene.SetActive(true);
        // PlayerInputController.SetLock(true);
        delaySceneTransition = StartCoroutine(DelayMoveScene(openingTime, FlowScene.Dojo_StartSlice));
    }

    void Play_Dojo_StartSlice()
    {
        narrator.PlayAudio(FlowScene.Dojo_StartSlice);
        titleScene.SetActive(false);
        PlayerInputController.SetLock(false);

        foreach (LauncherScript launcherScript in dojoLaunchers)
        {
            launcherScript.SetFire(true);
        }

        waitScoreSceneTransition =
            StartCoroutine(WaitForScoreMoveScene(goldenCarrotScore, FlowScene.Dojo_SpawnHarderCarrot));
    }

    /// <summary>
    /// Spawn golden carrot
    /// </summary>
    void Play_Dojo_SpawnHarderCarrot()
    {
        narrator.PlayAudio(FlowScene.Dojo_SpawnHarderCarrot);
        goldenLauncher.SetFire(true);
    }

    /// <summary>
    /// called when golden carrot is sliced
    /// </summary>
    void Play_Dojo_FirstUpgrade()
    {
        PlayerSliceController.SetPlayerLevel(SliceLevel.KatanaLong);
        float duration = narrator.PlayAudio(FlowScene.Dojo_FirstUpgrade);
        StartCoroutine(DelayPlayerLock(duration));
        waitScoreSceneTransition = StartCoroutine(WaitForScoreMoveScene(bambooScore, FlowScene.Dojo_SlicedBamboo));
    }

    void Play_Dojo_SlicedBamboo()
    {
        narrator.PlayAudio(FlowScene.Dojo_SlicedBamboo);
    }

    /// <summary>
    /// called when golden bamboo is sliced
    /// </summary>
    void Play_Dojo_Launchers()
    {
        float duration = narrator.PlayAudio(FlowScene.Dojo_Launchers);
        PlayerSliceController.SetPlayerLevel(SliceLevel.KatanaSuper);
        goldenLauncher.gameObject.SetActive(false);
        foreach (LauncherScript launcherScript in dojoLaunchers)
        {
            launcherScript.gameObject.SetActive(false);
        }

        sliceableLaunchers.SetActive(true);
        StartCoroutine(DelayPlayerLock(launcherSpeakTime));
    }

    void Play_Dojo_Crowbar()
    {
        narrator.PlayAudio(FlowScene.Dojo_Crowbar);
        PlayerSliceController.SetPlayerLevel(SliceLevel.Crowbar);
        studio.SetActive(true);
        player.transform.position = studioTeleportPoint.position;
        preyCamera.gameObject.SetActive(true);

        StartCoroutine(DelayPlayerLock(launcherSpeakTime));
    }

    void Play_Studio_Start()
    {
        float d = narrator.PlayAudio(FlowScene.Studio_Start);
        StartCoroutine(DelayActiveGameObject(d,fallingSpotLight)); 
    }

    void Play_Studio_LightSaber()
    {
        float d = narrator.PlayAudio(FlowScene.Studio_LightSaber);
        PlayerSliceController.SetPlayerLevel(SliceLevel.LightSaber);
        preyCamera.gameObject.SetActive(false);
        StartCoroutine(DelayPlayerLock(d*1));

    }

    void Play_Studio_Couch()
    {
        float d = narrator.PlayAudio(FlowScene.Studio_Couch);
        PlayerInputController.SetLock(true);
        PlayerInputController.SetLock(false);


    }

    void Play_Studio_Collapse()
    {
        narrator.Stop();
        studioAnimator.SetTrigger("Collapse");
    }

    void Play_Studio_Beg()
    {
        float d = narrator.PlayAudio(FlowScene.Studio_Collapse);
        // DelayPlayerLock(d);
        PlayerInputController.SetLock(true);

        delaySceneTransition = StartCoroutine(DelayMoveScene(d, FlowScene.Studio_DoIt));

    }
    void Play_Studio_DoIt()
    {
        studioAnimator.SetTrigger("BegEnd");
    }
    void Play_Studio_DoubleSaber()
    {
        float d = narrator.PlayAudio(FlowScene.Studio_Beg);
        PlayerInputController.SetLock(false);
        PlayerSliceController.SetPlayerLevel(SliceLevel.DualSaber);
    }
    void Play_Studio_End()
    {
        narrator.Stop();
        copyRightScreen.SetActive(true);
        SoundManager.current.StopAllSounds();
        PlayerInputController.SetLock(true);
        PlayerInputController.SetHide(true);

        elevatorMusic.Play();
        delaySceneTransition = StartCoroutine(DelayMoveScene(copyRightTime, FlowScene.Bsod_Start));

    }

    void Play_Bsod_Start()
    {
        Cursor.visible = false;
        elevatorMusic.Stop();
        errorSound.Play();
        copyRightScreen.SetActive(true);
        player.transform.position = bsodTeleportPoint.position;
        delaySceneTransition = StartCoroutine(DelayMoveScene(errorTime, FlowScene.Bsod_Display));
    }

    void Play_Bsod_Display()
    {
        copyRightScreen.SetActive(false);
        PlayerInputController.SetLock(true);
        PlayerInputController.SetHide(true);

        errorSound.Stop();
        bsodAnimator.SetTrigger("Start");
        offButton.SetActive(false);
        delaySceneTransition = StartCoroutine(DelayMoveScene(errorToNarrateTime, FlowScene.Bsod_Narrate));

    }

    void Play_Bsod_Narrate()
    {
        float t = narrator.PlayAudio(FlowScene.Bsod_Narrate);
        delaySceneTransition = StartCoroutine(DelayMoveScene(t+30f, FlowScene.Bsod_End));

    }

    void Play_Bsod_End()
    {
        Cursor.visible = true;

        PlayerInputController.SetLock(false);
        PlayerInputController.SetHide(false);
        offButton.SetActive(true);
        float t = narrator.PlayAudio(FlowScene.Bsod_End);

        PlayerSliceController.SetPlayerLevel(SliceLevel.Vindows);

    }

    void Play_MB_Start()
    {
        PlayerSliceController.SetPlayerLevel(SliceLevel.Vindows);
        player.transform.position = MBTeleportPoint.position;
        float t = narrator.PlayAudio(FlowScene.MB_Start);
        delaySceneTransition = StartCoroutine(DelayMoveScene(t, FlowScene.MB_BugRemoval));

    }

    void Play_MB_BugRemoval()
    {
        PlayerSliceController.SetPlayerLevel(SliceLevel.IBixIt);
        motherboardFilter.SetActive(true);
        float t = narrator.PlayAudio(FlowScene.MB_BugRemoval);

    }

    IEnumerator DelayStartOpening()
    {
        yield return new WaitForSeconds(2f);
        Play_Scene(FlowScene.Dojo_Opening);
    }


    IEnumerator DelayMoveScene(float time, FlowScene scene)
    {
        if (delaySceneTransition != null)
        {
            StopCoroutine(delaySceneTransition);
        }

        yield return new WaitForSeconds(time);
        Play_Scene(scene);
    }

    IEnumerator DelayPlayerLock(float t)
    {
        PlayerInputController.SetLock(true);

        yield return new WaitForSeconds(t);
        PlayerInputController.SetLock(false);
    }

    IEnumerator DelayActiveGameObject(float t,GameObject g)
    {

        yield return new WaitForSeconds(t);
        g.SetActive(true);
    }
    

    IEnumerator WaitForScoreMoveScene(int score, FlowScene scene)
    {
        if (waitScoreSceneTransition != null)
        {
            StopCoroutine(waitScoreSceneTransition);
        }

        yield return new WaitUntil(() => ScoreManager.Score > score);
        Play_Scene(scene);
    }

    public void OnSkipToStudio()
    {
        Play_Scene(FlowScene.Dojo_Crowbar);
    }

    public void OnSkipToCopyRight()
    {
        Play_Scene(FlowScene.Studio_End);
    }
    public void OnSkipToBsod()
    {
        Play_Scene(FlowScene.Bsod_Start);
    }

    public void OnSkipToMotherboard()
    {
        Play_Scene(FlowScene.MB_Start);
    }
    public void ReducePillar()
    {
        narratorRoomPillarCount -= 1;
        if (narratorRoomPillarCount <= 0)
        {
            Play_Scene(FlowScene.Studio_Collapse);
        }
    }
}