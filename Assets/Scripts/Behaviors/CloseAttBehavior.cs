using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CloseAttBehavior : BaseBehavior
{
    private string attName;

    public override void StartBehavior(Mob mob)
    {
        base.StartBehavior(mob);

        attName = (string)data;
        mob.Anim.SetTrigger(attName);
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
        //mob.Anim.ResetTrigger(attName);

        //ESword bMob = mob as ESword;
        //if (bMob && bMob.Target.IsDeath())
        //{
        //    bMob.ClearTarget();
        //}
    }
}
