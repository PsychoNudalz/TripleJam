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

    [Header("Damage")]
    [SerializeField]
    private float damageMultiplier = 1f;

    private float damage;

    private LayerMask layerMask;

    [SerializeField]
    private float damageRange = 5f;

    [SerializeField]
    private float damageOverTime_Duration = 0;

    [SerializeField]
    private float damageOverTime_Multiplier = 1f;

    [Space(10)]
    [SerializeField]
    private UnityEvent collisionEvent;

    [Space(10)]
    [SerializeField]
    private float delayDestroy = 5f;

    [SerializeField]
    private float disableColliderTime = 0.2f;

    [SerializeField]
    private Collider mainCollider;

    [SerializeField]
    private Collider damageTrigger;

    private List<LifeSystem> inTriggerUnit = new List<LifeSystem>();


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

        if (damageTrigger)
        {
            damageTrigger.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (inTriggerUnit.Count > 0)
        {
            foreach (LifeSystem lifeSystem in inTriggerUnit)
            {
                DamageSystem.DealDamage(lifeSystem,
                    damage * damageMultiplier * damageOverTime_Multiplier * Time.deltaTime);
            }
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
            rb.AddForce(velocity, ForceMode.VelocityChange);
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
        DamageSystem.SphereCastDamage(transform.position, damage * damageMultiplier, damageRange, layerMask);
        if (damageOverTime_Duration > 0f)
        {
            StartCoroutine(DelayTrigger());
        }

        rb.isKinematic = true;
        Destroy(gameObject, delayDestroy+damageOverTime_Duration);
    }

    IEnumerator DelayCollider()
    {
        mainCollider.enabled = false;
        yield return new WaitForSeconds(disableColliderTime);
        mainCollider.enabled = true;
    }

    IEnumerator DelayTrigger()
    {
        damageTrigger.enabled = true;
        yield return new WaitForSeconds(damageOverTime_Duration);
        damageTrigger.enabled = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out LifeSystem ls))
        {
            if (!inTriggerUnit.Contains(ls))
            {
                inTriggerUnit.Add(ls);
            }
        }

        LifeSystem[] temp = inTriggerUnit.ToArray();
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] == null)
            {
                inTriggerUnit.Remove(temp[i]);
            }
        }

        inTriggerUnit = new List<LifeSystem>(temp);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out LifeSystem ls))
        {
            if (inTriggerUnit.Contains(ls))
            {
                inTriggerUnit.Remove(ls);
            }
        }
    }
}