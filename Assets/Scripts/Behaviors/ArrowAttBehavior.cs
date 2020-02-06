using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAttBehaviorData
{
    public Transform target;
    public float lifeTime;

    public ArrowAttBehaviorData(Transform target, float lifeTime)
    {
        this.target = target;
        this.lifeTime = lifeTime;
    }
}
public class ArrowAttBehavior : BaseBehavior
{
    private ArrowAttBehaviorData mData;
    private float lifeTime;

    public override void StartBehavior(Mob mob)
    {
        base.StartBehavior(mob);

        mob.Anim.SetTrigger("att");

        mData = (ArrowAttBehaviorData)data;
        lifeTime = mData.lifeTime;
    }

    public override bool UpdateBehavior(Mob mob)
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime <= 0)
        {
            return false;
        }

        Vector3 lookDir = mData.target.position - mob.transform.position;
        lookDir.y = 0;
        lookDir.Normalize();
        mob.transform.LookAt(mData.target, Vector3.up);

        return true;
    }

    public override void EndBehavior(Mob mob)
    {
        base.EndBehavior(mob);


        EBow bMob = mob as EBow;
        if (bMob && bMob.Target.IsDeath())
        {
            bMob.ClearTarget();
        }
    }
}
