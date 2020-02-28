using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeBehavior : BaseBehavior
{
    protected LinkedList<BaseBehavior> behaviors = new LinkedList<BaseBehavior>();

    public override void StartBehavior(Mob mob)
    {
        base.StartBehavior(mob);
    }

    public override bool UpdateBehavior(Mob mob)
    {
        if (behaviors.Count>0)
        {
            BaseBehavior curBehavior = behaviors.First.Value;

            if (curBehavior.State == BehaviorState.NotStarted)
                curBehavior.StartBehavior(mob);

            if (!(curBehavior.UpdateBehavior(mob)))
            {
                curBehavior.EndBehavior(mob);
                behaviors.RemoveFirst();
            }

            return true;
        }

        return false;
    }

    protected void Add(Type type, System.Object data, BehaviorPriority priority, float lifeTime)
    {
        this.priority = priority;

        BaseBehavior newBehavior = ScriptableObject.CreateInstance(type) as BaseBehavior;
        newBehavior.Init(priority, data, lifeTime);
        behaviors.AddLast(newBehavior);
    }

    
}
