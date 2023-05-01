using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.Serialization;

public class Sliceable : MonoBehaviour
{
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

    public void Init(MeshRenderer r, MeshFilter f, Rigidbody rb = null)
    {
        meshRenderer = r;
        meshFilter = f;
        if (!rb && rigidbody)
        {
            rigidbody.velocity = rb.velocity;
            rigidbody.angularVelocity = rb.angularVelocity;
        }
    }

    public void Init(Mesh m, Rigidbody rb, float multiplier = 1)
    {
        SetMesh(m);
        rigidbody.velocity = rb.velocity*multiplier;
        rigidbody.angularVelocity = rb.angularVelocity*multiplier;
    }


    public void SetMesh(Mesh m)
    {
        meshFilter.mesh = m;
        meshCollider.sharedMesh = m;
    }
}