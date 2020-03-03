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

    protected BehaviorPriority priority;
    protected BehaviorState state= BehaviorState.NotStarted;
    
    public BehaviorPriority Priority { get { return priority; } }
    public BehaviorState State { get { return state; } }

    public virtual void Init(BehaviorPriority p, System.Object data, float lifeTime)
    {
        this.priority = p;

        //각 행동에 필요한 데이터를 Object형으로 일반화해서 가져온다
        //제너릭 타입을 가져와서 필요한 타입으로 캐스팅해서 사용
        this.data = data;

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
