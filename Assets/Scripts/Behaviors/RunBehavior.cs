using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunBehaviorData
{
    public Vector3 dest;

    public RunBehaviorData(Vector3 dest)
    {
        this.dest = dest;
    }
}

public class RunBehavior : BaseBehavior
{
    RunBehaviorData mData;

    public override void StartBehavior(Mob mob) {

        base.StartBehavior(mob);

        mob.Anim.SetTrigger("run");
        mob.Nav.isStopped = false;
        mData = (RunBehaviorData)data;
    }

    public override bool UpdateBehavior(Mob mob)
    {
        mob.Nav.destination = mData.dest;
        Vector3 targetDir = (mData.dest - mob.transform.position).normalized;
        Vector3 curDir = mob.transform.forward;
        mob.transform.forward = Vector3.Lerp(curDir, targetDir, 0.05f);

        Vector3 subPos = mob.transform.position - mData.dest;

        return (subPos.sqrMagnitude > 0.1f);
    }

    public override void EndBehavior(Mob mob)
    {
        base.EndBehavior(mob);
        mob.Anim.ResetTrigger("run");
        mob.Nav.isStopped = true;
        mob.Nav.destination = mob.transform.position;
    }

}
