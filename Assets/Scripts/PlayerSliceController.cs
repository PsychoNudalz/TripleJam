using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
[Serializable]
public struct SliceSet
{
    public SliceLevel level;
    public float length;
    public GameObject swordObject;

    public SliceSet(SliceLevel level, float length, GameObject swordObject)
    {
        this.level = level;
        this.length = length;
        this.swordObject = swordObject;
    }
}
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
    [SerializeField]
    private SliceSet[] sliceSets;


    private Vector3 bladePos;

    [SerializeField]
    private Vector2 mouseAngle_Offset;

    [Space(10)]
    [SerializeField]
    private Transform holsterPoint;
    [Space(10)]
    [SerializeField]
    private bool isDraw = false;

    [Header("Blade")]
    [SerializeField]
    private SliceBlade playerBlade;

    public static PlayerSliceController current;

    private void Awake()
    {
        if (visualBlade)
        {
            bladePos = visualBlade.position-transform.position;
        }

        current = this;
    }

    private void Start()
    {
        SetPlayerLevel(SliceLevel.None);
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
            visualBlade.transform.position = bladePos+transform.position;
            mainSlicer.gameObject.SetActive(true);
            visualBlade.transform.forward = (mouseTip.position-bladePos+transform.position).normalized;

        }
        else
        {
            visualBlade.transform.position = holsterPoint.position;
            visualBlade.transform.forward = holsterPoint.forward;
            mainSlicer.gameObject.SetActive(false);
        }
    }
    
    public static void SetPlayerLevel(SliceLevel sl)
    {
        current.SetPlayerBlade(sl);
        UIManager.current.UpdateBladeText(sl.ToString());
    }

    public void SetPlayerBlade(SliceLevel sl)
    {

        SliceSet set = GetSet(sl);
        playerBlade.SetPlayerBlade(sl,set.length);
        SetBladeMode(set.swordObject);
    }

    public SliceSet GetSet(SliceLevel sl)
    {
        foreach (SliceSet sliceSet in sliceSets)
        {
            if (sliceSet.level.Equals(sl))
            {
                return sliceSet;
            }
        }

        return sliceSets[0];
    }

    public void SetBladeMode(GameObject model)
    {
        foreach (SliceSet sliceSet in sliceSets)
        {
            sliceSet.swordObject.SetActive(false);
        }
        model.SetActive(true);
    }
}