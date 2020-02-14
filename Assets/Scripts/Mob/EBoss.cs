using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBoss : NPC
{
    private WeaponTrail trail;
    private BaseBehavior idleBehavior;
    private BaseBehavior dieBehavior;

    public override void GetDamaged(float amount)
    {
        // die this time
        if (curHP > 0 && curHP <= amount)
        {
            BaseBehavior deathBehavior = ScriptableObject.CreateInstance<DieBehavior>();
            deathBehavior.Init(BehaviorPriority.Vital, 0, null);
            fsm.AddBehavior(deathBehavior);
        }

        curHP -= amount;
    }

    public override void Start()
    {
        base.Start();

        trail = GetComponentInChildren<WeaponTrail>();
      
        idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
        idleBehavior.Init(BehaviorPriority.Basic, 0, null);
    }

    public override void AE_StartAttack()
    {
        trail.StartTrail();
    }
    public override void AE_EndAttack()
    {
        trail.EndTrail();
    }


    private void Update()
    {
        if (IsDeath())
            return;

        UpdateHPBar();
        UpdateTarget("Alley", ref target);

        if (target==null)
        {
            if (!fsm.ContainBehavior(Type.GetType("IdleBehavior")))
            {
                BaseBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                idleBehavior.Init(BehaviorPriority.Basic, 0, null);
                fsm.AddBehavior(idleBehavior);
            }
        }
    }


}

