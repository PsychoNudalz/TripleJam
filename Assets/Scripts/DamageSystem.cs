using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


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


    public virtual void DealDamage(LifeSystem ls, float damage)
    {
        ls.TakeDamage(damage);
    }

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
        ls.TakeDamage(damage);
    }
}