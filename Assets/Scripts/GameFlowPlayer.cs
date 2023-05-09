using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowPlayer : MonoBehaviour
{
    [SerializeField]
    private FlowScene flowScene = FlowScene.None;

    public void Play()
    {
        GameFlowManager.current.Play_Scene(flowScene);
    }

    [ContextMenu("Shift next")]
    public void ShiftNextScene()
    {
        flowScene += 1;
    }
}
