using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class LaserBehaviorData
{
    public bool isStartLaser = false;
    public Transform target;

    public LaserBehaviorData(Transform target)
    {
        this.target = target;
    }
}

public class LaserBehavior : BaseBehavior
{
    private LaserBehaviorData mData;

    public override void StartBehavior(Mob mob)
    {
        base.StartBehavior(mob);

        mData = (LaserBehaviorData)data;
        mob.Anim.SetTrigger("laser");
    }

    public override bool UpdateBehavior(Mob mob)
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            return false;

        Vector3 targetDir = (mData.target.position - mob.transform.position).normalized;
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
            mob.transform.Rotate(Vector3.up, radSpeed*Mathf.Sign(signDotV) *Time.deltaTime);
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
