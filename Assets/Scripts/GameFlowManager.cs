using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField]
    private int currentFlowPoint = 0;

    [SerializeField]
    private List<UnityEvent> flowEvents;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveFlowPoint(int i)
    {
        currentFlowPoint = i;
        if (i < flowEvents.Count)
        {
            flowEvents[i].Invoke();
        }
    }
}
