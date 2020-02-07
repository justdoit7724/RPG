using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FSM : MonoBehaviour
{
    // push back / pop front
    LinkedList<BaseBehavior> behaviors = new LinkedList<BaseBehavior>();
    Mob mob;

    public string CurBehaviorName()
    {
        if (behaviors.Count == 0)
            return null;

        return (CurBehavior().GetType().ToString());
    }
    public bool CanAdd(BehaviorPriority priority)
    {
        if (behaviors.Count == 0)
            return true;

        return (
            (LastBehavior().Priority < priority) ||
            ((LastBehavior().Priority == priority) && !LastBehavior().IsAlone)
            );
    }
    public void AddBehavior(BaseBehavior behavior)
    {
        if (behaviors.Count == 0)
        {
            behaviors.AddLast(behavior);
        }
        else if(LastBehavior() < behavior)
        {
            bool isAdd = false;
            while (LastBehavior() && LastBehavior() < behavior)
            {
                isAdd = true;
                if (LastBehavior().State == BehaviorState.InProcess)
                    LastBehavior().EndBehavior(mob);
                behaviors.RemoveLast();
            }
            if(isAdd)
                behaviors.AddLast(behavior);
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
                CurBehavior().EndBehavior(mob);
                behaviors.RemoveFirst();
            }
        }
    }

    private void OnGUI()
    {
        Vector2 scn = Camera.main.WorldToScreenPoint(transform.position);
        scn.y = Screen.height - scn.y;
        bool isFirst = false;
        foreach(var i in behaviors)
        {
            if(!isFirst)
            {
                GUI.color = Color.red;
                isFirst = true;
            }
            else
            {
                GUI.color = Color.cyan;
            }
            GUI.Label(new Rect(scn, new Vector2(150, 30)), i.ToString());

            scn.y += 20;
        }
    }
}
