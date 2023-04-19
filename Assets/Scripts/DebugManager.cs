using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{

    [SerializeField]
    private ComputerWork computerWork;
    public void OnStartDesktopGame()
    {
        if (computerWork)
        {
            computerWork.On_WorkComputer_Enter();
        }
    }
    
    public void OnEndDesktopGame()
    {
        if (computerWork)
        {
            computerWork.On_WorkComputer_Exit();
        }
    }
    
}
