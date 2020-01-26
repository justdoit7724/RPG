using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseBehavior : ScriptableObject
{
    protected System.Object data;

    public virtual void Start() { }

    public abstract void Update();

    public virtual void End() { }
}
