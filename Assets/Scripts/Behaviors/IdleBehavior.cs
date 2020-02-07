using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehavior : BaseBehavior
{

    public override void StartBehavior(Mob mob)
    {
        base.StartBehavior(mob);

        mob.Anim.SetTrigger("idle");
        mob.Nav.isStopped = true;
        mob.Nav.destination = mob.transform.position;
    }

    public override bool UpdateBehavior(Mob mob)
    {
        return true;
    }

    public override void EndBehavior(Mob mob)
    {
        base.EndBehavior(mob);
        mob.Anim.ResetTrigger("idle");
    }
}
