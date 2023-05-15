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

    [SerializeField]
    private bool canSliceOnce = false;

    [SerializeField]
    private bool originSideRemainStill = true;

    [SerializeField]
    private float minimumMeshRatio = 0.2f;

    private float originalMeshSize = -1;
    private float distanceToOrigin = 100;
    [Header("Components")]
    [SerializeField]
    private Rigidbody rigidbody;

    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private MeshFilter meshFilter;

    [SerializeField]
    private MeshCollider meshCollider;
    [SerializeField]
    GameObject[] deactivateObjects;



    public float DistanceToOrigin => distanceToOrigin;

    public Mesh mesh => meshFilter?.mesh;
    public Material[] materials => meshRenderer?.materials;

    public string name => gameObject.name;

    public Rigidbody Rigidbody => rigidbody;

    public const float minMeshSize = .01f;
    public const float remainMeshSize = 1f;

    public bool CanSliceOnce => canSliceOnce;

    public bool OriginSideRemainStill => originSideRemainStill;

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

        originSideRemainStill = originSideRemainStill && rigidbody.isKinematic;
        if (originalMeshSize < 0)
        {
            originalMeshSize = GetMeshSize(meshFilter.mesh);
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
    public void Init(Mesh m, Rigidbody rb, float multiplier = 1, bool beSlice = true)
    {
        SelfInit();
        if (!SetMesh(m))
        {
            return;
        }
        if (rigidbody)
        {
            rigidbody.isKinematic = false;
            rigidbody.velocity = rb.velocity * multiplier;
            rigidbody.angularVelocity = rb.angularVelocity * multiplier;
        }

        sliceScore /= 2;

        float meshSize = GetMeshSize(m);
        if (meshSize < minMeshSize)
        {
            Destroy(gameObject, 1f);
        }

        foreach (GameObject deactivateObject in deactivateObjects)
        {
            deactivateObject.SetActive(false);
        }

        _canSlice = beSlice;
        if (meshSize < remainMeshSize)
        {
            Destroy(gameObject, autoDestroyTime);
        }

        if (meshSize < originalMeshSize * minimumMeshRatio)
        {
            Destroy(gameObject, autoDestroyTime);
        }
        else
        {
            if (originSideRemainStill)
            {
                distanceToOrigin = GetDistanceToOrigin(mesh);
            }
        }
    }

    float GetMeshSize(Mesh m)
    {
        Vector3 boundsSize = m.bounds.size;
        return boundsSize.x * boundsSize.y * boundsSize.z;
    }


    public bool SetMesh(Mesh m)
    {
        meshFilter.mesh = m;
        try
        {
            meshCollider.sharedMesh = m;

        }
        catch (Exception e)
        {
            Destroy(gameObject);
            Debug.LogError($"{gameObject} set mesh FAILED");
            return false;
        }

        return true;
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

    float GetDistanceToOrigin(Mesh m)
    {
        Vector3 sum = default;
        foreach (Vector3 pos in m.vertices)
        {
            sum += pos;
        }

        sum /= m.vertices.Length;

        return Vector3.Distance(transform.localPosition, sum);
    }

    public void SetStill()
    {
        originSideRemainStill = true;
        rigidbody.isKinematic = true;
    }

    public void SetLose()
    {
        originSideRemainStill = false;
        rigidbody.isKinematic = false;
    }
    
    

}