using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CurseBehavior : CompositeBehavior
{
    public void Init(Vector3 firstPos, Vector3 secondPos, Transform target, float laserTime, BehaviorPriority p)
    {
        Add(Type.GetType("AnimEventBehavior"), "curse", p, false, 2.0f);
        Add(Type.GetType("RunBehavior"), new RunBehaviorData(firstPos), p, true,0.0f);
        Add(Type.GetType("LaserBehavior"), target, p, false, laserTime);
        Add(Type.GetType("RunBehavior"), new RunBehaviorData(secondPos), p, true, 0.0f);
        Add(Type.GetType("AnimEventBehavior"), "ball", p, false, 2.0f);
        Add(Type.GetType("AnimEventBehavior"), "curse", p, false, 2.0f);
    }
}
