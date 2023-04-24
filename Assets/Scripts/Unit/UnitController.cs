using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField]
    private UnitAIController aiController;

    [SerializeField]
    private WaypointMovement movement;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMove()
    {
        movement.Move_NewPoint_FromPos(WaypointController.main);
    }
}
