using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowPlayer : MonoBehaviour
{
    [SerializeField]
    private FlowScene flowScene = FlowScene.None;

    [SerializeField]
    private bool force = false;

    [SerializeField]
    private bool triggeredOnce = false;

    private bool trigger = false;

    public void Play()
    {
        if (triggeredOnce && trigger)
        {
            return;
        }

        if (triggeredOnce && !trigger)
        {
            GameFlowManager.current.Play_Scene(flowScene,force);
            trigger = true;
        }
        else
        {
            if (!triggeredOnce)
            {
                GameFlowManager.current.Play_Scene(flowScene,force);
            }
        }
    }

    [ContextMenu("Shift next")]
    public void ShiftNextScene()
    {
        flowScene += 1;
    }
}
