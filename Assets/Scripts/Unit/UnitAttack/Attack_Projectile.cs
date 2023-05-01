using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Attack_Projectile : UnitAttack
{
    [FormerlySerializedAs("projectile")]
    [Header("Projectile")]
    [SerializeField]
    private DamageProjectile damageProjectile;


    [SerializeField]
    private float projectileSpeed= 1;

    [SerializeField]
    private Vector3 launchOffset = new Vector3(0, 0.5f, 0);


    public override void OnAttack_Enter(UnitController target)
    {
        base.OnAttack_Enter(target);
    }

    public override void OnAttack_Action(UnitController target)
    {
        base.OnAttack_Action(target);
        DamageProjectile newDamageProjectile = Instantiate(damageProjectile, transform.position+launchOffset, Quaternion.identity)
            .GetComponent<DamageProjectile>();
        
        newDamageProjectile.Init(damage,layerMask, unitController);
        Vector3 trajectory = FindTrajectoryToTarget(target.Position, transform.position);
        newDamageProjectile.LaunchVelocity(trajectory);
    }

    Vector3 FindTrajectoryToTarget(Vector3 targetPos, Vector3 launchPos, bool flipYAngle = false)
    {
        Vector3 distance = targetPos - launchPos;
        float t = distance.magnitude/projectileSpeed;
        float horizontal = (distance.magnitude / t);
        float gravityMagnitude = -(Physics.gravity.y);
        float y = gravityMagnitude * t / 2f + distance.y;
        Vector3 velocity = new Vector3(horizontal * distance.normalized.x, y, horizontal * distance.normalized.z);

        // Debug.Log($"Calculated Velocity: {velocity}, {velocity.magnitude}");


        return velocity;
    }
}