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
    [Tooltip("The empty game object located at the tip of the blade")]
    private GameObject _tip = null;

    [SerializeField]
    [Tooltip("The empty game object located at the base of the blade")]
    private GameObject _base = null;

    [SerializeField]
    private float sliceLength = 0;


    [SerializeField]
    [Tooltip("The amount of force applied to each side of a slice")]
    private float _forceAppliedToCut = 3f;

    [SerializeField]
    private BoxCollider bladeCollider;

    [SerializeField]
    private Sliceable detectedSliceObject;

    [SerializeField]
    private Vector3 initialSliceObjectPosition;

    [Header("Effects")]
    [SerializeField]
    private ParticleSystem sliceEffect;

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
        if (s)
        {
            sliceEffect.Play();

        }
        else
        {
            detectedSliceObject = null;
            return;
        }
        if (SliceObject(other, s))
        {
        }

        detectedSliceObject = null;
    }

    private bool SliceObject(Collider collider, Sliceable s)
    {
        if (s)
        {
            if (!s.CanSlice(bladeLevel, _triggerEnterBasePosition, _triggerEnterTipPosition, _triggerExitTipPosition,
                    sliceLength))
            {
                return false;
            }
        }
        
        
        Plane plane = new Plane();
        Vector3 transformedNormal = new Vector3();
        if (s.Equals(detectedSliceObject))
        {
            Vector3 objectDisplacement = detectedSliceObject.transform.position - initialSliceObjectPosition;

            plane = GetPlane(collider, out transformedNormal, objectDisplacement);
        }
        else
        {
            plane = GetPlane(collider, out transformedNormal, Vector3.zero);
        }
        // plane = GetPlane(other, out transformedNormal , Vector3.zero);


        GameObject[] slices = Array.Empty<GameObject>();

        slices = Slicer.Slice(plane, s);


        if (slices.Length > 0)
        {
            Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
            if (rigidbody)
            {
                Vector3 newNormal = transformedNormal * _forceAppliedToCut;
                rigidbody.AddForce(newNormal, ForceMode.Impulse);
            }

            rigidbody = slices[0].GetComponent<Rigidbody>();
            if (rigidbody)
            {
                Vector3 newNormal = -transformedNormal * _forceAppliedToCut;
                rigidbody.AddForce(newNormal, ForceMode.Impulse);
            }
        }

        return slices.Length > 0;
    }

    private Plane GetPlane(Collider other, out Vector3 transformedNormal, Vector3 objectDisplacement)
    {
        _triggerExitTipPosition = _tip.transform.position;

        //Create a triangle between the tip and base so that we can get the normal
        Vector3 side1 = _triggerExitTipPosition - _triggerEnterTipPosition + objectDisplacement;
        Vector3 side2 = _triggerExitTipPosition - _triggerEnterBasePosition;

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
        if (sl.Equals(SliceLevel.None))
        {
            bladeLevel = SliceLevel.None;
            return true;
        }
        if (sl > bladeLevel)
        {
            bladeLevel = sl;
            return true;
        }

        return false;
    }

    public void SetPlayerBlade(SliceLevel sl, float length)
    {
        sliceLength = length;

        if (!SetPlayerLevel(sl))
        {
            return;
        }

        // Vector3 bladeColliderSize = bladeCollider.size;
        // bladeColliderSize.z = length;
        // bladeCollider.size = bladeColliderSize;
        //
        // var bladeColliderCenter = bladeCollider.center;
        // bladeColliderCenter.z = length / 2f;
        // bladeCollider.center = bladeColliderCenter;
        Vector3 transformLocalScale = transform.localScale;
        transformLocalScale.z = length;
        transform.localScale = transformLocalScale;
    }
}