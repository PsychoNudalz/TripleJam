using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

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

    [Header("Resources")]
    [SerializeField]
    private float resources;

    [SerializeField]
    private int resourcePerSecond;

    [Header("Components")]
    [SerializeField]
    private PlayerInputController playerInputController;

    [Header("Spawning")]
    [SerializeField]
    private bool autoSpawn = false;

    [SerializeField]
    private float spawnTime = 2f;
    float spawnTime_Now = 0;
    
    [SerializeField]
    private LayerMask floorLayer;

    [SerializeField]
    private Transform spawnZone;

    [SerializeField]
    private Transform target;

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
        if (autoSpawn)
        {
            spawnTime_Now -= Time.deltaTime;
            if (spawnTime_Now < 0)
            {
                spawnTime_Now = spawnTime;

                Vector3 randomSpawn = GetRandomSpawn();
            
                if (SpawnUnit(Random.Range(0, units.Length),randomSpawn,GetAdjacentPosition(randomSpawn)))
                {
                }
            }
        }
    }


    private void FixedUpdate()
    {
        resources += resourcePerSecond * Time.deltaTime;
    }

    public void OnUnit_1(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            SpawnUnit(0, transform.position,WaypointController.main.position);
        }
    }

    /// <summary>
    /// true if spawns
    /// </summary>
    /// <param name="i"></param>
    /// <param name="pos"></param>
    /// <param name="force"></param>
    /// <returns></returns>
    bool SpawnUnit(int i, Vector3 pos, Vector3 targetPos, bool force = false)
    {
        playerInputController.UpdateWaypointToCursor();
        UnitController unit = units[i];

        if (!force && unit.Cost > resources)
        {
            return false;
        }

        unit = Instantiate(unit.gameObject, pos, quaternion.identity).GetComponent<UnitController>();
        unit.SetTargetPos(targetPos, transform.forward);
        resources -= unit.Cost;
        return true;
    }

    Vector3 GetRandomSpawn()
    {
        if (!spawnZone)
        {
            return transform.position;
        }
        Vector3 spawnZoneLocalScale = spawnZone.localScale;
        Vector3 randomPos = new Vector3(Random.Range(-spawnZoneLocalScale.x, spawnZoneLocalScale.x), 0,
            Random.Range(-spawnZoneLocalScale.z, spawnZoneLocalScale.z) / 2f) + spawnZone.position;
        randomPos = transform.rotation * randomPos;
        if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 100,floorLayer))
        {
            randomPos = hit.point;
        }
        return randomPos;
    }

    Vector3 GetAdjacentPosition(Vector3 start)
    {
        Vector3 dir = target.position - transform.position;
        return start + dir;

    }
}