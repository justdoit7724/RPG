﻿using System;
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
        lifeTime -= Time.deltaTime;

        return (lifeTime>0);
    }

    public override void EndBehavior(Mob mob)
    {
        base.EndBehavior(mob);
        //mob.Nav.isStopped = false;
    }
}
