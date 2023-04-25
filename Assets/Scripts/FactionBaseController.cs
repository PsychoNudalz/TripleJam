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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnUnit1(InputValue inputValue)
    {
        
    }

    void SpawnUnit(int i)
    {
        UnitController unit = units[i];
        unit = Instantiate(unit, transform.position, quaternion.identity);
        unit.SetTargetPos(WaypointController.main.position,transform.forward);
    }
}
