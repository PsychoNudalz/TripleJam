using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HeadDetector))]

public class HeadDetectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Reset Picture"))
        {
            HeadDetector myScript = (HeadDetector) target;

            myScript.MarkFacePoints();
            myScript.UpdateRenderer();
        }
        
        if (GUILayout.Button("Take Picture"))
        {
            HeadDetector myScript = (HeadDetector) target;
            
            myScript.TakePlayerPicture_HeadImage();
        }

    }
}
