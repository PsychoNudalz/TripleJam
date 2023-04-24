using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Projectile : UnitAttack
{
    [Header("Projectile")]
    [SerializeField]
    private Projectile projectile;


    [SerializeField]
    private float projectileTime = 1;


    public override void OnAttack_Enter(UnitController target)
    {
        base.OnAttack_Enter(target);
    }

    public override void OnAttack_Action(UnitController target)
    {
        base.OnAttack_Action(target);
        Projectile newProjectile = Instantiate(projectile, transform.position, Quaternion.identity)
            .GetComponent<Projectile>();
        
        newProjectile.Init(damage,layerMask);
        Vector3 trajectory = FindTrajectoryToTarget(target.Position, transform.position);
        newProjectile.LaunchVelocity(trajectory);
    }

    Vector3 FindTrajectoryToTarget(Vector3 targetPos, Vector3 launchPos, bool flipYAngle = false)
    {
        Vector3 distance = targetPos - launchPos;
        float t = projectileTime;
        float horizontal = (distance.magnitude / t);
        float gravityMagnitude = -(Physics.gravity.y);
        float y = gravityMagnitude * t / 2f + distance.y;
        Vector3 velocity = new Vector3(horizontal * distance.normalized.x, y, horizontal * distance.normalized.z);

        Debug.Log($"Calculated Velocity: {velocity}, {velocity.magnitude}");


        return velocity;
    }
}