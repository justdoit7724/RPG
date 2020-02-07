using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttBehaviorData
{
    public float lifeTime;
    public string name;

    public SwordAttBehaviorData(string name, float lifeTime)
    {
        this.name = name;
        this.lifeTime = lifeTime;
    }
}

public class SwordAttBehavior : BaseBehavior
{
    SwordAttBehaviorData mData;
    private float lifeTime=1;

    public override void StartBehavior(Mob mob)
    {
        base.StartBehavior(mob);

        mData = (SwordAttBehaviorData)data;
        mob.Anim.SetTrigger(mData.name);
        lifeTime = mData.lifeTime;
    }

    public override bool UpdateBehavior(Mob mob)
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            return false;

        return true;
    }

    public override void EndBehavior(Mob mob)
    {
        base.EndBehavior(mob);
        mob.Anim.ResetTrigger(mData.name);

        ESword bMob = mob as ESword;
        if (bMob && bMob.Target.IsDeath())
        {
            bMob.ClearTarget();
        }
    }
}
