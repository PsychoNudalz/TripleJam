using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;

public class LauncherScript : MonoBehaviour
{
    [SerializeField]
    protected GameObject launchGO;

    [SerializeField]
    protected float speed = 200;

    [SerializeField]
    protected bool fireOnAwake = false;
    [SerializeField]
    protected float fireDelay = 1f;
    


    private void Start()
    {
        if (fireOnAwake)
        {
            StartCoroutine(FireDelay());
        }
    }

    [ContextMenu("Fire")]
    public virtual void OnFire()
    {
        Rigidbody rb = GameObject.Instantiate(launchGO,transform.position,Quaternion.identity).GetComponent<Rigidbody>();
        rb.velocity = (transform.forward * speed);
        Destroy(rb.gameObject, 10f);
    }
    

    IEnumerator FireDelay()
    {
        yield return new WaitForSeconds(fireDelay);
        OnFire();
    }
}