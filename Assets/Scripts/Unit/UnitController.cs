using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    enum UnitFaction
    {
        Attacker,
        Defender
    }
    [SerializeField]
    private UnitAIController aiController;

    [SerializeField]
    private WaypointMovement movement;

    [Header("States")]
    [SerializeField]
    private UnitFaction faction = UnitFaction.Defender;

    public Vector3 Position => transform.position;
    
    public bool IsFriendly(UnitController other)
    {
        return other.faction.Equals(faction);
    }
    
    public bool IsHostile(UnitController other)
    {
        return !other.faction.Equals(faction);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMove(WaypointController newPoint = null) 
    {
        if (!newPoint)
        {
            newPoint = WaypointController.main;
        }
        movement.Move_NewPoint_FromPos(newPoint);
    }

    public bool IsMoving => movement.IsMoving;

}
