using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


[CustomEditor(typeof(WaypointManager))]
public class WaypointManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WaypointManager myScript = (WaypointManager) target;
        if (GUILayout.Button("Set Waypoints"))
        {
            myScript.SetWaypoints();
            SetDirty(myScript);
        }

        if (GUILayout.Button("Auto Assign By Closest"))
        {
            myScript.AddByClosest();
            SetDirty(myScript);
        }


        if (GUILayout.Button("Set Waypoints by Distance "))
        {
            myScript.AddByDistance(myScript.Editor_Distance);
            SetDirty(myScript);
        }

        GUILayout.Space(10f);
        if (GUILayout.Button("Reset Waypoints"))
        {
            myScript.ResetWaypoints();
            SetDirty(myScript);
        }
    }

    public void SetDirty(WaypointManager manager)
    {
        EditorUtility.SetDirty(manager.gameObject);
        foreach (WaypointController wp in manager.Waypoints)
        {
            EditorUtility.SetDirty(wp.gameObject);
            EditorUtility.SetDirty(wp);
            Debug.Log($"{wp.gameObject} mark dirty");
        }
    }
}


// [CustomEditor(typeof(WaypointController))]
// public class WaypointEditor : Editor
// {
//     private float distance = 5f;
//
//     public override void OnInspectorGUI()
//     {
//         WaypointController myScript = (WaypointController) target;
//         if (GUILayout.Button("Reset Waypoints"))
//         {
//             myScript.ResetConnected();
//             EditorUtility.SetDirty(myScript.gameObject);
//         }
//
//         if (GUILayout.Button("Set Waypoints by Distance "))
//         {
//             myScript.AddWaypoint_Distance(distance);
//             EditorUtility.SetDirty(myScript.gameObject);
//         }
//
//
//         DrawDefaultInspector();
//     }
// }