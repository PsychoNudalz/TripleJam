using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[Serializable]
public enum SliceLevel
{
    None,
    KatanaLong,
    KatanaSuper,
    Crowbar,
    LightSaber,
    DualSaber,
    CheeseKnife,
    DataKnife,
    CorruptedKnife
}


public class Sliceable : MonoBehaviour
{
    [SerializeField]
    private bool _canSlice = true;

    [SerializeField]
    private SliceLevel sliceLevel = SliceLevel.None;

    [SerializeField]
    private UnityEvent onSliceEvent;

    [SerializeField]
    private int sliceScore = 0;


    [Header("Settings")]
    [SerializeField]
    private bool _isSolid = true;

    [SerializeField]
    private bool _reverseWindTriangles = false;

    [SerializeField]
    private bool _useGravity = false;

    [SerializeField]
    private bool _shareVertices = false;

    [SerializeField]
    private bool _smoothVertices = false;

    [SerializeField]
    private float autoDestroyTime = 10f;

    [Header("Components")]
    [SerializeField]
    private Rigidbody rigidbody;

    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private MeshFilter meshFilter;

    [SerializeField]
    private MeshCollider meshCollider;


    public Mesh mesh => meshFilter?.mesh;
    public Material[] materials => meshRenderer?.materials;

    public string name => gameObject.name;

    public Rigidbody Rigidbody => rigidbody;

    public const float minMeshSize = .01f;

    public bool CanSlice()
    {
        return _canSlice;
    }

    public bool CanSlice(SliceLevel sliceLevel)
    {
        return _canSlice && CheckLevel(sliceLevel);
    }

    public bool IsSolid
    {
        get { return _isSolid; }
        set { _isSolid = value; }
    }

    public bool ReverseWireTriangles
    {
        get { return _reverseWindTriangles; }
        set { _reverseWindTriangles = value; }
    }

    public bool UseGravity
    {
        get { return _useGravity; }
        set { _useGravity = value; }
    }

    public bool ShareVertices
    {
        get { return _shareVertices; }
        set { _shareVertices = value; }
    }

    public bool SmoothVertices
    {
        get { return _smoothVertices; }
        set { _smoothVertices = value; }
    }

    private void Awake()
    {
        SelfInit();
    }

    public void SelfInit()
    {
        if (!meshRenderer)
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        if (!meshFilter)
        {
            meshFilter = GetComponentInChildren<MeshFilter>();
        }

        if (!meshCollider)
        {
            meshCollider = GetComponentInChildren<MeshCollider>();
        }

        if (!rigidbody)
        {
            rigidbody = GetComponent<Rigidbody>();
        }
    }

    // public void Init(MeshRenderer r, MeshFilter f, Rigidbody rb = null)
    // {
    //     meshRenderer = r;
    //     meshFilter = f;
    //     if (!rb && rigidbody)
    //     {
    //         rigidbody.velocity = rb.velocity;
    //         rigidbody.angularVelocity = rb.angularVelocity;
    //     }
    //
    //     Destroy(gameObject, autoDestroyTime);
    // }

    /// <summary>
    /// Use this one
    /// </summary>
    /// <param name="m"></param>
    /// <param name="rb"></param>
    /// <param name="multiplier"></param>
    public void Init(Mesh m, Rigidbody rb, float multiplier = 1)
    {
        SelfInit();
        SetMesh(m);
        if (rigidbody)
        {
            rigidbody.isKinematic = false;
            rigidbody.velocity = rb.velocity * multiplier;
            rigidbody.angularVelocity = rb.angularVelocity * multiplier;
        }

        sliceScore /= 2;

        if (GetMeshSize(m) < minMeshSize)
        {
            Destroy(gameObject, 1f);
        }

        Destroy(gameObject, autoDestroyTime);
    }

    float GetMeshSize(Mesh m)
    {
        Vector3 boundsSize = m.bounds.size;
        return boundsSize.x * boundsSize.y * boundsSize.z;
    }


    public void SetMesh(Mesh m)
    {
        meshFilter.mesh = m;
        meshCollider.sharedMesh = m;
    }

    public bool CheckLevel(SliceLevel l)
    {
        return l >= sliceLevel;
    }

    public void OnSlice_Spawn()
    {
        onSliceEvent.Invoke();
    }

    public void OnSlice_Destroy()
    {
        ScoreManager.AddScore(sliceScore);
    }

}