using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ProjectileLauncher : LauncherScript
{
    [Header("Projectile")]
    [SerializeField]
    private Projectile projectile;
    // Start is called before the first frame update

    private void Awake()
    {
        if (projectile)
        {
            launchGO = projectile.gameObject;
        }
    }

    public override void OnFire()
    {
        Projectile p = Instantiate(projectile.gameObject, transform.position, quaternion.identity)
            .GetComponent<Projectile>();
        p.LaunchVelocity(transform.forward*speed);
    }
}
