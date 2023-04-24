using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private float damageMultiplier = 1f;

    private float damage;

    private LayerMask layerMask;

    [SerializeField]
    private float damageRange = 5f;

    [SerializeField]
    private UnityEvent collisionEvent;

    [SerializeField]
    private float delayDestroy = 5f;

    [SerializeField]
    private float disableColliderTime = 0.2f;

    [SerializeField]
    private Collider mainCollider;

    public float Mass => rb.mass;

    private void Awake()
    {
        if (!rb)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (!mainCollider)
        {
            mainCollider = GetComponent<Collider>();
        }
    }

    public virtual void Init(float baseDamage, LayerMask layerMask)
    {
        damage = baseDamage;
        this.layerMask = layerMask;
    }

    public virtual void LaunchVelocity(Vector3 velocity)
    {
        if (rb)
        {
            rb.AddForce(velocity,ForceMode.VelocityChange);
        }
        else
        {
            Debug.LogWarning($"{this} missing RB!");
        }

        StartCoroutine(DelayCollider());
    }

    private void OnCollisionEnter(Collision other)
    {
        OnCollisionBehaviour();
    }

    public virtual void OnCollisionBehaviour()
    {
        collisionEvent.Invoke();
        DamageSystem.SphereCastDamage(transform.position,damage*damageMultiplier,damageRange,layerMask);
        Destroy(gameObject,delayDestroy);
    }

    IEnumerator DelayCollider()
    {
        mainCollider.enabled = false;
        yield return new WaitForSeconds(disableColliderTime);
        mainCollider.enabled = true;
    }
}
