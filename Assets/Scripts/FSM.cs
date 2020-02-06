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

    public bool AddBehavior(BaseBehavior behavior)
    {
        if (behaviors.Count == 0)
        {
            behaviors.AddLast(behavior);
            return true;
        }
        else
        {
            bool isAdd = false;
            while (LastBehavior() && LastBehavior() < behavior)
            {
                isAdd = true;
                if (LastBehavior().state == BehaviorState.InProcess)
                    LastBehavior().EndBehavior(mob);
                behaviors.RemoveLast();
            }
            if(isAdd)
                behaviors.AddLast(behavior);

            return isAdd;
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
