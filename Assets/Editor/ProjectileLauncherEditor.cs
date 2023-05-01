using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [CustomEditor(typeof(ProjectileLauncher))]

public class ProjectileLauncherEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ProjectileLauncher myScript = (ProjectileLauncher) target;
        if (GUILayout.Button("Fire"))
        {
            myScript.OnFire();
            SetDirty(myScript);
        }

    }

    public void SetDirty(ProjectileLauncher launcher)
    {
        EditorUtility.SetDirty(launcher.gameObject);

    }
}
