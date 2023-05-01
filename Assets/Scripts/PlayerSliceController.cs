using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSliceController : MonoBehaviour
{
    [FormerlySerializedAs("mainRotationalObject")]
    [SerializeField]
    private Transform mainSlicer;

    [SerializeField]
    private Transform mouseTip;

    [FormerlySerializedAs("visualRotationalObject")]
    [SerializeField]
    private Transform visualBlade;

    private Vector3 bladePos;

    [SerializeField]
    private Vector2 mouseAngle_Offset;

    [Space(10)]
    [SerializeField]
    private Transform holsterPoint;
    [Space(10)]
    [SerializeField]
    private bool isDraw = false;

    private void Awake()
    {
        if (visualBlade)
        {
            bladePos = visualBlade.position;
        }
    }

    public void UpdateController(Vector3 worldPos, float angle)
    {
        if (!isDraw)
        {
            return;
        }
        UpdateSlicer(worldPos, angle);
        UpdateVisual(worldPos, angle);
        UpdateMouseTip(worldPos, angle);
    }

    public void UpdateSlicer(Vector3 worldPos, float angle)
    {
        if (mainSlicer)
        {
            UpdateRotationObject(mainSlicer, worldPos, angle);
        }
    }

    public void UpdateVisual(Vector3 worldPos, float angle)
    {
        if (visualBlade)
        {
            UpdateRotationObject(visualBlade, worldPos, angle);
        }
    }

    void UpdateRotationObject(Transform t, Vector3 worldPos, float angle)
    {
        Vector3 dir = worldPos - t.position;
        mouseAngle_Offset.x = -Mathf.Atan(dir.y / dir.z) * Mathf.Rad2Deg;
        mouseAngle_Offset.y = Mathf.Atan(dir.x / dir.z) * Mathf.Rad2Deg;
        Vector3 transformEulerAngles = t.eulerAngles;
        transformEulerAngles.x = mouseAngle_Offset.x;
        transformEulerAngles.y = mouseAngle_Offset.y;
        transformEulerAngles.z = angle;

        t.eulerAngles = transformEulerAngles;
    }

    void UpdateMouseTip(Vector3 pos, float rotationz)
    {
        if (!mouseTip)
        {
            return;
        }

        mouseTip.position = pos;
        Vector3 transformEulerAngles = mouseTip.eulerAngles;
        transformEulerAngles.z = rotationz;

        mouseTip.eulerAngles = transformEulerAngles;
    }

    public void SetDraw(bool b)
    {
        isDraw = b;
        if (isDraw)
        {
            visualBlade.transform.position = bladePos;
            mainSlicer.gameObject.SetActive(true);
            visualBlade.transform.forward = (mouseTip.position-bladePos).normalized;

        }
        else
        {
            visualBlade.transform.position = holsterPoint.position;
            visualBlade.transform.forward = holsterPoint.forward;
            mainSlicer.gameObject.SetActive(false);
        }
    }
}