using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAttBehavior : BaseBehavior
{
    private Transform target;

    public override void StartBehavior(Mob mob)
    {
        base.StartBehavior(mob);

        mob.Anim.SetTrigger("att");

        target = (Transform)data;
    }

    public override bool UpdateBehavior(Mob mob)
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            return false;
        }

        Vector3 lookDir = target.position - mob.transform.position;
        lookDir.y = 0;
        lookDir.Normalize();
        mob.transform.LookAt(target, Vector3.up);

        return true;
    }
}
