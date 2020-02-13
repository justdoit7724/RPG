using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GolemJumpBehavior : CompositeBehavior
{
    public void Init(Vector3 dest)
    {
        Add(Type.GetType("RunBehavior"), new RunBehaviorData(dest), 0, BehaviorPriority.Skill);
    }
}
