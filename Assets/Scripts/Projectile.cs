using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    enum ProjectileState
    {
        Launch,
        Collide
    }

    ProjectileState projectileState = ProjectileState.Launch;

    [SerializeField]
    private Rigidbody rb;

    [FormerlySerializedAs("damageMultiplier")]
    [Header("Damage")]
    [SerializeField]
    private float damageCollision_Multiplier = 1f;

    private float damage;

    [SerializeField]
    private LayerMask damageLayerMask;

    [SerializeField]
    private bool singleDamage = false;

    [FormerlySerializedAs("damageRange")]
    [SerializeField]
    private float damageCollision_Range = 5f;

    [SerializeField]
    private AnimationCurve damageCurve;

    [Space(5)]
    [SerializeField]
    private float damageOverTime_Duration = 0;

    [SerializeField]
    private float damageOverTime_Multiplier = 1f;

    [SerializeField]
    private float damageOverTime_Range = 0f;


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

    [SerializeField]
    private Transform zoneDisplay;

    private List<LifeSystem> inTriggerUnit = new List<LifeSystem>();

    private UnitController sourceUnit;

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
            if (damageTrigger is SphereCollider sc)
            {
                sc.radius = damageOverTime_Range;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (projectileState == ProjectileState.Collide)
        {
            Gizmos.DrawSphere(transform.position, damageOverTime_Range);
        }
    }

    private void FixedUpdate()
    {
        if (inTriggerUnit.Count > 0)
        {
            foreach (LifeSystem lifeSystem in inTriggerUnit)
            {
                DamageSystem.DealDamage(lifeSystem,
                    new DamageData(damage * damageOverTime_Multiplier * Time.fixedDeltaTime, transform.position,
                        damageOverTime_Range));
            }
        }
    }

    public virtual void Init(float baseDamage, LayerMask layerMask, UnitController source)
    {
        damage = baseDamage;
        damageLayerMask = layerMask;
        sourceUnit = source;
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

    private void OnCollisionEnter(Collision other)
    {
        OnCollisionBehaviour();
    }

    public virtual void OnCollisionBehaviour()
    {
        collisionEvent.Invoke();
        DamageSystem.SphereCastDamage(transform.position, damage * damageCollision_Multiplier, damageCollision_Range,
            damageLayerMask, damageCurve, singleDamage);
        if (damageOverTime_Duration > 0f)
        {
            StartCoroutine(DelayTrigger());
        }

        rb.isKinematic = true;
        projectileState = ProjectileState.Collide;
        transform.rotation = Quaternion.identity;
        Destroy(gameObject, delayDestroy + damageOverTime_Duration);
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
        if (zoneDisplay)
        {
            zoneDisplay.localScale = new Vector3(damageOverTime_Range * 2f, zoneDisplay.localScale.y,
                damageOverTime_Range * 2f);
        }

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