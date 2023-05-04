using System;
using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceBlade : MonoBehaviour
{
    [SerializeField]
    private SliceLevel bladeLevel = SliceLevel.None;

    [SerializeField]
    private bool isPlayer = false;

    [SerializeField]
    [Tooltip("The empty game object located at the tip of the blade")]
    private GameObject _tip = null;

    [SerializeField]
    [Tooltip("The empty game object located at the base of the blade")]
    private GameObject _base = null;


    [SerializeField]
    [Tooltip("The amount of force applied to each side of a slice")]
    private float _forceAppliedToCut = 3f;

    [SerializeField]
    private BoxCollider bladeCollider;

    [SerializeField]
    private Sliceable detectedSliceObject;

    [SerializeField]
    private Vector3 initialSliceObjectPosition;

    private Mesh _mesh;
    private Vector3[] _vertices;
    private int[] _triangles;
    private int _frameCount;
    private Vector3 _previousTipPosition;
    private Vector3 _previousBasePosition;
    private Vector3 _triggerEnterTipPosition;
    private Vector3 _triggerEnterBasePosition;
    private Vector3 _triggerExitTipPosition;


    private void Awake()
    {
    }

    void Start()
    {
    }

    private void Update()
    {
    }

    void LateUpdate()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (detectedSliceObject)
        {
            return;
        }
        Sliceable temp = other.GetComponentInParent<Sliceable>();
        if (temp)
        {
            detectedSliceObject = temp;
            _triggerEnterTipPosition = _tip.transform.position;
            _triggerEnterBasePosition = _base.transform.position;
            initialSliceObjectPosition = detectedSliceObject.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Sliceable s = other.GetComponentInParent<Sliceable>();

        if (!s.Equals(detectedSliceObject))
        {
            return;
        }

        Vector3 objectDisplacement = detectedSliceObject.transform.position -initialSliceObjectPosition;
        
        var plane = GetPlane(other, out var transformedNormal,objectDisplacement);

        GameObject[] slices = Array.Empty<GameObject>();
        if (s)
        {
            if (s.CanSlice(bladeLevel))
            {
                slices = Slicer.Slice(plane, s);
            }
        }
        else
        {
            slices = Slicer.Slice(plane, other.gameObject);
        }

        if (slices.Length > 0)
        {
            Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
            if (rigidbody)
            {
                Vector3 newNormal = transformedNormal + Vector3.up * _forceAppliedToCut;
                rigidbody.AddForce(newNormal, ForceMode.Impulse);
            }
        }

        detectedSliceObject = null;
    }

    private Plane GetPlane(Collider other, out Vector3 transformedNormal, Vector3 objectDisplacement)
    {
        _triggerExitTipPosition = _tip.transform.position;

        //Create a triangle between the tip and base so that we can get the normal
        Vector3 side1 = _triggerExitTipPosition - _triggerEnterTipPosition;
        Vector3 side2 = _triggerExitTipPosition - _triggerEnterBasePosition+objectDisplacement;

        //Get the point perpendicular to the triangle above which is the normal
        //https://docs.unity3d.com/Manual/ComputingNormalPerpendicularVector.html
        Vector3 normal = Vector3.Cross(side1, side2).normalized;

        //Transform the normal so that it is aligned with the object we are slicing's transform.
        transformedNormal = ((Vector3) (other.gameObject.transform.localToWorldMatrix.transpose * normal)).normalized;

        //Get the enter position relative to the object we're cutting's local transform
        Vector3 transformedStartingPoint = other.gameObject.transform.InverseTransformPoint(_triggerEnterTipPosition);

        Plane plane = new Plane();

        plane.SetNormalAndPosition(
            transformedNormal,
            transformedStartingPoint);

        var direction = Vector3.Dot(Vector3.up, transformedNormal);

        //Flip the plane so that we always know which side the positive mesh is on
        if (direction < 0)
        {
            plane = plane.flipped;
        }

        return plane;
    }

    public bool SetPlayerLevel(SliceLevel sl)
    {
        if (sl > bladeLevel)
        {
            bladeLevel = sl;
            return true;
        }

        return false;
    }

    public void SetPlayerBlade(SliceLevel sl, float length)
    {
        if (!SetPlayerLevel(sl))
        {
            return;
        }

        Vector3 bladeColliderSize = bladeCollider.size;
        bladeColliderSize.z = length;
        bladeCollider.size = bladeColliderSize;

        var bladeColliderCenter = bladeCollider.center;
        bladeColliderCenter.z = length / 2f;
        bladeCollider.center = bladeColliderCenter;
    }
}