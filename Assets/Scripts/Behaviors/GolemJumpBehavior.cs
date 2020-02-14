using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GolemJumpBehavior : CompositeBehavior
{
    private const float jumpMaxDist = 5.0f;

    public void Init(Vector3 dest, Vector3 curPos, BehaviorPriority p)
    {
        Vector3 subPos = dest - curPos;
        float sqrJumpMaxDist = jumpMaxDist * jumpMaxDist;
        if(subPos.sqrMagnitude> sqrJumpMaxDist)
        {
            Vector3 jumpStartPt = dest - subPos.normalized * jumpMaxDist;
            Add(Type.GetType("RunBehavior"), new RunBehaviorData(jumpStartPt), 0, p);
        }
        Add(Type.GetType("JumpBehavior"), new JumpBehaviorData(dest, 1), 0, p);
        Add(Type.GetType("IdleBehavior"), null, 3, p);
    }
}
