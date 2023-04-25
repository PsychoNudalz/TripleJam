using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

enum UnitFaction
{
    Attacker,
    Defender
}
public class FactionBaseController : MonoBehaviour
{
    [SerializeField]
    private UnitFaction faction;

    [SerializeField]
    private UnitController[] units;

    [SerializeField]
    private PlayerInputController playerInputController;

    private void Awake()
    {
        if (!playerInputController)
        {
            playerInputController = GetComponent<PlayerInputController>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnUnit_1(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            SpawnUnit(0);
        }
    }

    void SpawnUnit(int i)
    {
        playerInputController.UpdateWaypointToCursor();
        UnitController unit = units[i];
        unit = Instantiate(unit.gameObject, transform.position, quaternion.identity).GetComponent<UnitController>();
        unit.SetTargetPos(WaypointController.main.position,transform.forward);
    }
}
