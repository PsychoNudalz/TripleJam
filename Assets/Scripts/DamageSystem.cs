using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[AllowsNull]
public struct DamageData
{
    private Vector3 point;
    private float range;
    private float damage;

    public Vector3 Point => point;
    public float Damage => damage;

    public float Range => range;


    public DamageData(float damage, Vector3 point, float range)
    {
        this.point = point;
        this.range = range;
        this.damage = damage;
    }
}

/// <summary>
/// to control damaging player
/// </summary>
public class DamageSystem : MonoBehaviour
{
    [SerializeField]
    protected float damagePerSecond;

    [SerializeField]
    protected string[] damageTags;

    [SerializeField]
    protected LayerMask damageLayers;

    [SerializeField]
    protected bool needLineOfSight = false;

    [SerializeField]
    private LayerMask allLayers;

    [SerializeField]
    protected float LOSRange = 0;


    public bool isLOS(LifeSystem target)
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, LOSRange, allLayers);
        if (hit)
        {
            if (hit.collider.TryGetComponent(out LifeSystem ls) && ls.Equals(target))
            {
                return true;
            }

            return false;
        }
        else
        {
            return true;
        }
    }

    public static bool isLOS(LifeSystem target, LifeSystem self, float LOSRange, LayerMask layerMask)
    {
        Vector3 dir = (target.transform.position - self.transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(self.transform.position, dir, LOSRange, layerMask);
        if (hit)
        {
            if (hit.collider.TryGetComponent(out LifeSystem ls) && ls.Equals(target))
            {
                return true;
            }
        }

        return false;
    }

    public static void DealDamage(LifeSystem ls, float damage, LifeSystem self = null)
    {
        ls.TakeDamage(damage, self);
    }

    public static void DealDamage(LifeSystem ls, DamageData damageData, LifeSystem self = null)
    {
        ls.TakeDamage(damageData, self);
    }

    public static void SphereCastDamage(Vector3 position, float damage, float range, LayerMask layerMask,
        LifeSystem self = null)
    {
        Collider[] colliders = Physics.OverlapSphere(position, range, layerMask);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out LifeSystem lifeSystem))
            {
                DealDamage(lifeSystem, new DamageData(damage, position, range), self);
            }
        }
    }

    public static void SphereCastDamage(Vector3 position, float damage, float range, LayerMask layerMask,
        AnimationCurve damageCurve, LifeSystem self = null)
    {
        Collider[] colliders = Physics.OverlapSphere(position, range, layerMask);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out LifeSystem lifeSystem))
            {
                float rangeScale = damageCurve.Evaluate((collider.transform.position - position).magnitude / range);
                DealDamage(lifeSystem, new DamageData(damage*rangeScale, position, range), self);
            }
        }
    }
}