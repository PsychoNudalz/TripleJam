using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum FlowScene
{
    None,
    Dojo_Opening,
    Dojo_StartSlice,
    Dojo_SpawnHarderCarrot,
    Dojo_FirstUpgrade,
    Dojo_SlicedBamboo,
    Dojo_Launchers,
    Dojo_Crowbar
}
public class GameFlowManager : MonoBehaviour
{
    [SerializeField]
    private FlowScene currentScene = FlowScene.None;

    [SerializeField]
    private List<UnityEvent> flowEvents;

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
    [Header("Components")]
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private NarratorManager narrator;

    public static GameFlowManager current;
    private Coroutine delaySceneTransition;


    private void Awake()
    {
        current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayStartOpening());
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

        if (flowScene <= currentScene&&!force)
        {
            return;
        }
        Debug.Log($"Playing {flowScene}");
        switch (flowScene)
        {
            case FlowScene.Dojo_Opening:
                Play_Dojo_Open();
                break;
            case FlowScene.Dojo_StartSlice:
                Play_Dojo_StartSlice();
                break;
            case FlowScene.Dojo_SpawnHarderCarrot:
                break;
            case FlowScene.Dojo_FirstUpgrade:
                break;
            case FlowScene.Dojo_SlicedBamboo:
                break;
            case FlowScene.Dojo_Launchers:
                break;
            case FlowScene.Dojo_Crowbar:
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
        PlayerInputController.SetLock(true);
        delaySceneTransition = StartCoroutine(DelayMoveScene(openingTime,FlowScene.Dojo_StartSlice));
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
        delaySceneTransition = StartCoroutine(DelayMoveScene(sliceStartTime,FlowScene.Dojo_StartSlice));

    }

     void Play_Dojo_SpawnHarderCarrot()
     {
         narrator.PlayAudio(FlowScene.Dojo_SpawnHarderCarrot);
        goldenLauncher.SetFire(true);
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
}
