using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LaserBehavior : BaseBehavior
{
    private Transform target=null;

    public override void StartBehavior(Mob mob)
    {
        base.StartBehavior(mob);

        if (data!=null)
        {
            target = (Transform)data;
        }
    }

    public override bool UpdateBehavior(Mob mob)
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            return false;

        if (target)
        {
            Vector3 targetDir = (target.position - mob.transform.position).normalized;
            float dotV = Vector3.Dot(targetDir, mob.transform.forward);
            float cosRad = Mathf.Acos(dotV);
            if (cosRad < (Mathf.PI * 0.25f))
            {
                mob.transform.forward = Vector3.Lerp(mob.transform.forward, targetDir.normalized, 0.075f);
            }
            else
            {
                float signDotV = Vector3.Dot(targetDir, mob.transform.right);
                const float radSpeed = 45.0f;
                mob.transform.Rotate(Vector3.up, radSpeed * Mathf.Sign(signDotV) * Time.deltaTime);
            }
        }

        return true;
    }

    public override void EndBehavior(Mob mob)
    {
        base.EndBehavior(mob);

        EBoss boss = mob as EBoss;
        boss.EndLaser();
    }
}
