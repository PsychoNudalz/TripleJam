using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowPlayer : MonoBehaviour
{
    [SerializeField]
    private FlowScene flowScene = FlowScene.None;

    [SerializeField]
    private bool force = false;

    public void Play()
    {
        
        GameFlowManager.current.Play_Scene(flowScene,force);
    }

    [ContextMenu("Shift next")]
    public void ShiftNextScene()
    {
        flowScene += 1;
    }
}
