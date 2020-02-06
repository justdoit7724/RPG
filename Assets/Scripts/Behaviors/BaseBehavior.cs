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

    private BehaviorPriority priority;
    private bool isAlone = true;

    public BehaviorState state= BehaviorState.NotStarted;


    public virtual void Init(BehaviorPriority p, System.Object data, bool isAlone)
    {
        this.priority = p;
        this.data = data;
        this.isAlone = isAlone;
    }

    public static bool operator >(BaseBehavior a, BaseBehavior b)
    {
        if(a.priority > b.priority)
        {
            return true;
        }
        else if(a.priority == b.priority)
        {
            if(!b.isAlone)
            {
                return true;
            }
        }

        return false;
    }
    public static bool operator <(BaseBehavior a, BaseBehavior b)
    {
        if ((a.priority < b.priority)|| (a.priority == b.priority && !a.isAlone))
        {
            return true;
        }

        return false;
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
