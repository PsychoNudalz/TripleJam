using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{    protected enum ProjectileState
    {
        Launch,
        Collide
    }

    protected ProjectileState projectileState = ProjectileState.Launch;

    [SerializeField]
    protected Rigidbody rb;
    
    [SerializeField]
    protected float disableColliderTime = 0.2f;

    [SerializeField]
    protected Collider mainCollider;

    [SerializeField]
    private float projectileLifeTime = -1;

    [SerializeField]
    private Vector3 extraForce = new Vector3();
    // Start is called before the first frame update

    private void Awake()
    {
        AwakeBehaviour();
    }

    protected virtual void AwakeBehaviour()
    {
        if (!rb)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (projectileLifeTime > 0)
        {
            Destroy(gameObject,projectileLifeTime);
        }
    }

    private void FixedUpdate()
    {
        if (extraForce.magnitude > 0)
        {
            rb.AddForce(extraForce,ForceMode.Acceleration);
        }
    }

    public virtual void LaunchVelocity(Vector3 velocity)
    {
        if (rb)
        {
            rb.AddForce(velocity, ForceMode.VelocityChange);
        }
        else
        {
            Debug.LogWarning($"{this} missing RB!");
        }

        projectileState = ProjectileState.Launch;
        StartCoroutine(DelayCollider());
    }
    
    IEnumerator DelayCollider()
    {
        mainCollider.enabled = false;
        yield return new WaitForSeconds(disableColliderTime);
        mainCollider.enabled = true;
    }
    

}
