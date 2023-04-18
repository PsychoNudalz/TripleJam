using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class WaypointController : MonoBehaviour
{
    [SerializeField]
    private WaypointController[] connected;

    [SerializeField]
    private WaypointManager manager;

    [Space(10f)]
    [SerializeField]
    private UnityEvent onMoveEvent;

    public WaypointManager Manager
    {
        get => manager;
        set => manager = value;
    }

    public int GizmosCounter
    {
        get => Gizmos_Counter;
        set => Gizmos_Counter = value;
    }

    public Vector3 position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public int Gizmos_Counter = 0;

    public Vector3 forward => transform.forward;

    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool Contains(WaypointController c)
    {
        return connected.Contains(c);
    }

    private void OnDrawGizmosSelected()
    {
        if (connected.Length > 0)
        {
            Gizmos.color = new Color((float) (((Gizmos_Counter + 1) * .2) % 1),
                (float) (((Gizmos_Counter + 3) * .2) % 1), (float) (((Gizmos_Counter + 5) * .2) % 1));
            Vector3 offset = new Vector3(0, (float) ((Gizmos_Counter * 0.01) % .3), 0);
            
            foreach (WaypointController waypointController in connected)
            {
                Gizmos.DrawLine(position+offset, waypointController.position+offset);
            }

        }
    }

    public void ResetConnected()
    {
        connected = Array.Empty<WaypointController>();
        Debug.Log($"{this} RESET");
    }

    public void SetConnected(WaypointController[] newConnected)
    {
        connected = newConnected;
    }

    public void AddWaypoint(WaypointController waypoint)
    {
        if (!connected.Contains(waypoint) && !waypoint.Equals(this))
        {
            List<WaypointController> temp = (new List<WaypointController>(connected));
            temp.Add(waypoint);
            connected = temp.ToArray();
            Debug.Log($"Assign {waypoint} to {this}");
        }
    }

    public void AddWaypoint_Distance(float d)
    {
        foreach (WaypointController waypointController in manager.Waypoints)
        {
            if (Vector3.Distance(position, waypointController.position) < d)
            {
                AddWaypoint(waypointController);
            }
        }
    }

    public WaypointController GetConnected(Vector3 dir)
    {
        if (connected.Length == 0)
        {
            Debug.LogError($"{this} missing connected");
            return null;
        }
        WaypointController closest = connected[0];
        double smallestDot = -2;
        foreach (WaypointController waypointController in connected)
        {
            double current = Vector3.Dot(dir, GetDir(waypointController));
            if (smallestDot < current)
            {
                closest = waypointController;
                smallestDot = current;
            }
        }

        return closest;
    }

    public Vector3 GetDir(WaypointController other)
    {
        return (other.position - position).normalized;
    }

    public void OnMove()
    {
        onMoveEvent.Invoke();
    }
}