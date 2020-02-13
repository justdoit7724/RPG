using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum BehaviorPriority
{
    None,
    Basic,
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

    protected BehaviorPriority priority;
    protected BehaviorState state= BehaviorState.NotStarted;
    
    public virtual List<BaseBehavior> Get()
    {
        List<BaseBehavior> list = new List<BaseBehavior>(1);
        list.Add(this);
        return list;
    }

    public BehaviorPriority Priority { get { return priority; } }
    public BehaviorState State { get { return state; } }

    public virtual void Init(BehaviorPriority p, float lifeTime, System.Object data)
    {
        this.priority = p;
        this.data = data;
        // forever if negative
        this.lifeTime = (lifeTime <= 0) ? float.MaxValue : lifeTime;
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
