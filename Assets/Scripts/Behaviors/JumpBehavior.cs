using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBehaviorData
{
    public Vector3 dest;
    public float airTime;

    public JumpBehaviorData(Vector3 dest, float airTime)
    {
        this.dest = dest;
        this.airTime = airTime;
    }
}

public class JumpBehavior : BaseBehavior
{
    private Vector3 firstPos;
    private JumpBehaviorData mData;
    private float curTime = 0;

    public override void StartBehavior(Mob mob)
    {
        base.StartBehavior(mob);

        mob.Anim.SetTrigger("jump");
        mData = (JumpBehaviorData)data;
        mob.Nav.enabled = false;
        firstPos = mob.transform.position;
        Vector3 subVec = mData.dest - firstPos;
        mob.transform.forward = subVec.normalized;
    }

    public override bool UpdateBehavior(Mob mob)
    {
        curTime += Time.deltaTime;

        float t = Mathf.Clamp01(curTime / mData.airTime);

        Vector3 curPos = Vector3.Lerp(firstPos, mData.dest, t);
        mob.transform.position = curPos;

        return (t < 1);
    }

    public override void EndBehavior(Mob mob)
    {
        base.EndBehavior(mob);

        mob.Nav.enabled = true;
        mob.Nav.destination = mob.transform.position;
    }
}
