using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRunBehaviorData
{
    public bool isRunning;
    public Vector3 dir = new Vector3(0,0,0);
    public float speed;

    public ManualRunBehaviorData(bool b, float speed)
    {
        isRunning=b;
        this.speed = speed;
    }
}

public class ManualRunBehavior : BaseBehavior
{
    ManualRunBehaviorData mData;

    public override void Init(BehaviorPriority p, object data, bool isAlone)
    {
        base.Init(p, data, isAlone);

        mData = (ManualRunBehaviorData)data;
    }

    public override void StartBehavior(Mob mob)
    {
        base.StartBehavior(mob);

        mob.Anim.SetBool("isRun", true);
    }

    public override bool UpdateBehavior(Mob mob)
    {
        mob.Rigid.velocity = mData.dir * mData.speed;

        mob.transform.LookAt(mob.transform.position + mData.dir, Vector3.up);

        return mData.isRunning;
    }

    public override void EndBehavior(Mob mob)
    {
        mob.Rigid.velocity = Vector3.zero;

        base.EndBehavior(mob);
    }
}
