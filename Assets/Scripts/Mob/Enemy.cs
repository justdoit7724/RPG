using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FSM))]
public class Enemy : Mob
{
    private FSM fsm;

    public void Start()
    {
        fsm = GetComponent<FSM>();
    }

    public void AddBehavior(BaseBehavior behavior)
    {
        fsm.AddBehavior(behavior);
    }
}
