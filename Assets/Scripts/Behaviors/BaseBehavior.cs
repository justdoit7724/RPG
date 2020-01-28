using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseBehavior : ScriptableObject
{
    protected System.Object data;

    public virtual void StartBehavior(Mob mob) { }

    public abstract void UpdateBehavior(Mob mob);

    public virtual void EndBehavior(Mob mob) { }
}
