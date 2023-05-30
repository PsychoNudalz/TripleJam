using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SliceableUnit : MonoBehaviour
{
    [SerializeField]
    private Sliceable sliceable;

    [SerializeField]
    private Movement movement;

    [SerializeField]
    private Transform movementZone;

    [SerializeField]
    private SliceableUnitManager manager;

    public Transform MovementZone
    {
        get => movementZone;
        set => movementZone = value;
    }

    public SliceableUnitManager Manager
    {
        get => manager;
        set => manager = value;
    }

    // Start is called before the first frame update

    private void Awake()
    {
        if (!movementZone)
        {
            movementZone = transform.parent;
        }

        if (!sliceable)
        {
            sliceable = GetComponentInChildren<Sliceable>();
        }
    }

    void Start()
    {
        transform.position = GetRandomPos();
    }

    // Update is called once per frame
    void Update()
    {
        if (!movement.IsMoving)
        {
            movement.MoveToTarget(GetRandomPos());
        // movement.MoveToTarget(GetRandomPos(),
        //         new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));
        }
    }

    Vector3 GetRandomPos()
    {
        Vector3 zone = movementZone.lossyScale;
        Vector3 pos = movementZone.position + new Vector3(Random.Range(-zone.x, zone.x), Random.Range(-zone.y, zone.y),
            Random.Range(-zone.z, zone.z)) / 2f;
        pos = movementZone.rotation * pos;
        return pos;
    }

    public void OnSlice()
    {
        if (manager)
        {
            manager.Remove(this);
        }
        enabled = false;
        Destroy(gameObject,1f);
    }
}