using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{
    Queue<BaseBehavior> behaviors;

    public void AddBehavior(BaseBehavior behavior)
    {
        behaviors.Enqueue(behavior);
    }

    void Start()
    {
    }

    void Update()
    {
        
    }
}
