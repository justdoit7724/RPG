using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GolemJumpBehavior : CompositeBehavior
{
    private const float jumpMaxDist = 4.0f;

    public void Init(Vector3 dest, Vector3 curPos, BehaviorPriority p)
    {
        Vector3 subPos = dest - curPos;
        float sqrJumpMaxDist = jumpMaxDist * jumpMaxDist;
        if(subPos.sqrMagnitude> sqrJumpMaxDist)
        {
            Vector3 jumpStartPt = dest - subPos.normalized * jumpMaxDist;
            Add(Type.GetType("RunBehavior"), new RunBehaviorData(jumpStartPt), p, false, 5.0f);
        }
        Add(Type.GetType("JumpBehavior"), new JumpBehaviorData(dest, 1), p, true, 0);
        Add(Type.GetType("IdleBehavior"), null, p, false, 3.0f);
    }
}
