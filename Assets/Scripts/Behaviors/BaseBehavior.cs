using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum BehaviorPriority
{
    None,
    Basic,
    Att,
    Skill,
    Vital
}
public enum BehaviorState
{
    NotStarted,
    InProcess,
    Ended
}
public abstract class BaseBehavior : ScriptableObject
{
    protected System.Object data=null;
    protected float lifeTime = 0;
    protected bool isNoTime = true;

    protected BehaviorPriority priority;
    protected BehaviorState state= BehaviorState.NotStarted;
    
    public BehaviorPriority Priority { get { return priority; } }
    public BehaviorState State { get { return state; } }

    public virtual void Init(BehaviorPriority p, System.Object data, bool noTime, float lifeTime=2.0f)
    {
        this.priority = p;
        this.data = data;
        // forever if negative
        this.lifeTime = lifeTime;
        this.isNoTime = noTime;
    }


    public virtual void StartBehavior(Mob mob)
    {
        if (priority == BehaviorPriority.None)
            Debug.LogError("Behavior priority is not set");

        state = BehaviorState.InProcess;
    }

    public abstract bool UpdateBehavior(Mob mob);

    public virtual void EndBehavior(Mob mob) {

        state = BehaviorState.Ended;
    }
}
