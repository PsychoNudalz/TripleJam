using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [SerializeField]
    private WaypointController[] waypoints;

    [SerializeField]
    private WaypointController startingPoint;

    [Header("Editor_Settings")]
    [SerializeField]
    private float editor_Distance = 5;

    [SerializeField]
    private bool crossAdd = true;


    public float Editor_Distance => editor_Distance;

    public WaypointController[] Waypoints => waypoints;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ResetWaypoints()
    {
        foreach (WaypointController waypointController in waypoints)
        {
            waypointController.ResetConnected();
            
        }
    }

    public void SetWaypoints()
    {
        waypoints = GetComponentsInChildren<WaypointController>();
        for (var i = 0; i < waypoints.Length; i++)
        {
            var waypointController = waypoints[i];
            waypointController.Manager = this;
            waypointController.Gizmos_Counter = i;
        }
    }

    public void AddByClosest()
    {
        foreach (WaypointController current in waypoints)
        {
            WaypointController closest = null;

            foreach (WaypointController controller in waypoints)
            {
                if (!current.Equals(controller))
                {
                    if (!closest)
                    {
                        closest = controller;
                    }
                    else
                    {
                        if (!current.Contains(controller))
                        {
                            if (Vector3.Distance(current.position, controller.position) <
                                Vector3.Distance(current.position, closest.position))
                            {
                                closest = controller;
                            }
                        }
                    }
                }
            }

            if (closest)
            {
                current.AddWaypoint(closest);
                closest.AddWaypoint(current);
            }
        }
    }

    public void AddByDistance(float d)
    {
        foreach (WaypointController waypointController in waypoints)
        {
            waypointController.AddWaypoint_Distance(d);
        }
    }
}