using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FSM : MonoBehaviour
{
    // push back / pop front
    private LinkedList<BaseBehavior> behaviors = new LinkedList<BaseBehavior>();
    private Dictionary<Type, int> behaviorKinds = new Dictionary<Type, int>();
    private Mob mob;

    public int Count { get { return behaviors.Count; } }

    public void Clear()
    {
        behaviors.Clear();
        behaviorKinds.Clear();
    }
    public bool ContainBehavior(Type type)
    {
        return behaviorKinds.ContainsKey(type);
    }

    public void CheckAndAddBehavior(BaseBehavior behavior)
    {
        while(LastBehavior() && (LastBehavior().Priority <= behavior.Priority))
        {
            RemoveLastBehavior();
        }

        behaviors.AddLast(behavior);
        if (behaviorKinds.ContainsKey(behavior.GetType()))
        {
            behaviorKinds[behavior.GetType()]++;
        }
        else
        {
            behaviorKinds.Add(behavior.GetType(), 1);
        }
    }
    public void DirectAddBehavior(BaseBehavior behavior)
    {
        behaviors.AddLast(behavior);
        if (behaviorKinds.ContainsKey(behavior.GetType()))
        {
            behaviorKinds[behavior.GetType()]++;
        }
        else
        {
            behaviorKinds.Add(behavior.GetType(), 1);
        }
    }
    private void RemoveFirstBehavior()
    {
        BaseBehavior firstBehavior = behaviors.First.Value;
        firstBehavior.EndBehavior(mob);
        behaviors.RemoveFirst();
        behaviorKinds[firstBehavior.GetType()]--;
        if (behaviorKinds[firstBehavior.GetType()] == 0)
            behaviorKinds.Remove(firstBehavior.GetType());
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
        behaviorKinds[lastBehavior.GetType()]--;
        if (behaviorKinds[lastBehavior.GetType()] == 0)
            behaviorKinds.Remove(lastBehavior.GetType());
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
        if(name =="Golem")
        {
            int a = 0;

        }

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

    //private void OnGUI()
    //{

    //    Vector2 scn = Camera.main.WorldToScreenPoint(transform.position);
    //    scn.y = Screen.height - scn.y;
    //    bool isFirst = false;
    //    foreach(var i in behaviors)
    //    {
    //        if(!isFirst)
    //        {
    //            GUI.color = Color.red;
    //            isFirst = true;
    //        }
    //        else
    //        {
    //            GUI.color = Color.cyan;
    //        }
    //        GUI.Label(new Rect(scn, new Vector2(150, 30)), i.ToString());

    //        scn.y += 20;
    //    }
    //}
}
