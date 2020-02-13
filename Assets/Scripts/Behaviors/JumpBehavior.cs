using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBehaviorData
{
    public Vector3 dest;
    public float maxHeight;
    public float airTime;

    public JumpBehaviorData(Vector3 dest, float maxHeight, float airTime)
    {
        this.dest = dest;
        this.maxHeight = maxHeight;
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
        firstPos = mob.transform.position;
    }

    public override bool UpdateBehavior(Mob mob)
    {
        curTime += Time.deltaTime;

        float t = Mathf.Clamp01(curTime / mData.airTime);
        float posT = 1 - Mathf.Pow(curTime - 1, 2);
        float heightT = 1 - Mathf.Pow(2*curTime - 1, 2);

        Vector3 curPos = Vector3.Lerp(firstPos, mData.dest, posT);
        curPos.y = heightT * mData.maxHeight;
        mob.transform.position = curPos;

        return (t < 1);
    }
}
