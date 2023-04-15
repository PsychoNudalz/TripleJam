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

        WaypointManager myScript = (WaypointManager)target;
        if (GUILayout.Button("Set Waypoints"))
        {
            myScript.SetWaypoints();
            EditorUtility.SetDirty(myScript.gameObject);
        }
        if (GUILayout.Button("Auto Assign By Closest"))
        {
            myScript.AddByClosest();
            EditorUtility.SetDirty(myScript.gameObject);

        }


        if (GUILayout.Button("Set Waypoints by Distance "))
        {
            myScript.AddByDistance(myScript.Editor_Distance);
            EditorUtility.SetDirty(myScript.gameObject);
        }
        
        GUILayout.Space(10f);
        if (GUILayout.Button("Reset Waypoints"))
        {
            myScript.ResetWaypoints();
            EditorUtility.SetDirty(myScript.gameObject);
        }
        

    }
    
}


[CustomEditor(typeof(WaypointController))]

public class WaypointEditor : Editor
{
    [Header("Settings")]
    [SerializeField]
    private float distance = 5f;
    public override void OnInspectorGUI()
    {

        WaypointController myScript = (WaypointController)target;
        if (GUILayout.Button("Reset Waypoints"))
        {
            myScript.ResetConnected();
            EditorUtility.SetDirty(myScript.gameObject);
        }
        if (GUILayout.Button("Set Waypoints by Distance "))
        {
            myScript.AddWaypoint_Distance(distance);
            EditorUtility.SetDirty(myScript.gameObject);
        }


        
        DrawDefaultInspector();

    }
    
}
