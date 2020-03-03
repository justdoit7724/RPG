using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FSM : MonoBehaviour
{
    // Last-In-First-Out 상태에서 삽입, 제거가 모두 효율적인 '링크드 리스트' 사용
    private LinkedList<BaseBehavior> behaviors = new LinkedList<BaseBehavior>();
    private Mob mob;

    public int Count { get { return behaviors.Count; } }

    public void Clear()
    {
        behaviors.Clear();
    }

    public void CheckAndAddBehavior(BaseBehavior behavior)
    {
        while(LastBehavior() && (LastBehavior().Priority <= behavior.Priority))
        {
            RemoveLastBehavior();
        }

        behaviors.AddLast(behavior);
    }
    public void DirectAddBehavior(BaseBehavior behavior)
    {
        behaviors.AddLast(behavior);
    }
    private void RemoveFirstBehavior()
    {
        BaseBehavior firstBehavior = behaviors.First.Value;
        firstBehavior.EndBehavior(mob);
        behaviors.RemoveFirst();
    }
    private void RemoveLastBehavior()
    {
        BaseBehavior lastBehavior = behaviors.Last.Value;
        switch (lastBehavior.State)
        {
            case BehaviorState.InProcess:
                lastBehavior.EndBehavior(mob);
                break;
        }

        behaviors.RemoveLast();
    }

    public Type CurBehaviorType {
        get {
            if (CurBehavior() == null)
                return null;
            return CurBehavior().GetType();
        }
    }
    private BaseBehavior CurBehavior()
    {
        if (behaviors.Count == 0)
            return null;

        return behaviors.First.Value;
    }
    private BaseBehavior NextBehavior()
    {
        if (behaviors.Count <= 1)
            return null;

        return behaviors.First.Next.Value;
    }
    private BaseBehavior LastBehavior()
    {
        if (behaviors.Count == 0)
            return null;

        return behaviors.Last.Value;
    }

    void Start()
    {
        mob = GetComponent<Mob>();
    }

    void Update()
    {
        // check
        if (CurBehavior())
        {
            if (CurBehavior().State == BehaviorState.NotStarted)
                CurBehavior().StartBehavior(mob);

            // update
            if (!(CurBehavior().UpdateBehavior(mob)))
            {
                RemoveFirstBehavior();
            }
        }
    }

}
