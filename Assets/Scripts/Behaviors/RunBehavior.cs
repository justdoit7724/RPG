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
    public override void Init(BehaviorPriority p, object data, bool isAlone)
    {
        base.Init(p, data, isAlone);
    }

    public override void StartBehavior(Mob mob) {

        base.StartBehavior(mob);

        mob.Anim.SetBool("isRun", true);
        mob.Nav.isStopped = false;
    }

    public override bool UpdateBehavior(Mob mob)
    {
        mob.Nav.destination=((RunBehaviorData)data).dest;

        return true;
    }

    public override void EndBehavior(Mob mob)
    {
        base.EndBehavior(mob);
        mob.Nav.isStopped = true;
    }

}
