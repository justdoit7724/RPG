using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieBehavior : BaseBehavior
{
    private float delayTime = 5;

    public override void StartBehavior(Mob mob)
    {
        base.StartBehavior(mob);

        mob.Anim.SetTrigger("die");
        mob.Nav.enabled = false;
        Collider coll = mob.GetComponent<Collider>();
        if (coll)
            coll.enabled = false;

        mob.enabled = false;

        MobMgr.Instance.SendMessage(mob, MobMessage.Die);
        MobMgr.Instance.RemoveMob(mob);

        mob.RemoveSounds();
    }

    public override bool UpdateBehavior(Mob mob)
    {
        delayTime -= Time.deltaTime;
        if (delayTime > 0.0f)
            return true;

        if (mob.transform.position.y > -3.0f)
        {
            mob.transform.position = mob.transform.position - mob.transform.up * Time.deltaTime * 0.25f;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void EndBehavior(Mob mob)
    {
        base.EndBehavior(mob);

        Destroy(mob.gameObject);
    }
}
