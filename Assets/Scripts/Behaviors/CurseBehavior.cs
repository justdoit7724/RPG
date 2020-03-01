using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CurseBehavior : CompositeBehavior
{
    public void Init(GameObject laserPrefab, Transform laserPt, float laserLength, float laserDamage, Vector3 firstPos, Vector3 secondPos, AudioSource audioPlayer, Mob target, float laserTime, float laserChargeTime, BehaviorPriority p)
    {
        Add(Type.GetType("AnimEventBehavior"), new AnimEventBData("curse"), p, 3.0f);
        Add(Type.GetType("RunBehavior"), new RunBehaviorData(firstPos), p,0.0f);
        Add(Type.GetType("AnimEventBehavior"), new AnimEventBData("laser", audioPlayer, "LaserCharge", 1.0f), p, laserChargeTime);
        Add(Type.GetType("LaserBehavior"), new LaserBData(target, audioPlayer, "Laser", 0.25f), p, laserTime+0.5f);
        Add(Type.GetType("RunBehavior"), new RunBehaviorData(secondPos), p, 0.0f);
        Add(Type.GetType("AnimEventBehavior"), new AnimEventBData("ball", audioPlayer, "BossBallAura", 0.5f), p, 2.0f);
        Add(Type.GetType("AnimEventBehavior"), new AnimEventBData("curse"), p, 3.0f);
    }
}
