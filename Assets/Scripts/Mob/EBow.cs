﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBow : NPC
{
    [SerializeField] protected float attRad = 15;
    [SerializeField] private GameObject visualArrow;
    [SerializeField] private float bowDamage = 30;
    protected float sqrAttRad;
    private Bow weapon;



    public Mob Target { get { return target; } }

    public override void Start()
    {
        base.Start();

        sqrAttRad = attRad * attRad;

        weapon = GetComponentInChildren<Bow>();
        weapon.SetDamage(bowDamage);
    }

    public override void AE_StartAttack()
    {
    }
    public void AE_Fire()
    {
        weapon.FireArrow();
        visualArrow.SetActive(false);
        PlayMainSound("BowAtt", 0.5f);
    }
    public override void AE_EndAttack()
    {
        visualArrow.SetActive(true);
    }

    

    void Update()
    {
        if (IsDeath() || !isUpdating)
            return;


        hpBar.UpdateBar(curHP, maxHP, transform.position);
        UpdateTarget("Alley", ref target);

        if (target == null)
        {
            if(fsm.CurBehaviorType != Type.GetType("IdleBehavior"))
            {
                BaseBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                idleBehavior.Init(BehaviorPriority.Basic, null, 0);
                fsm.CheckAndAddBehavior(idleBehavior);
            }
        }
        else
        {
            Vector3 subVec = transform.position - target.transform.position;
            if (subVec.sqrMagnitude <= sqrAttRad)
            {
                if (fsm.CurBehaviorType != Type.GetType("ArrowAttBehavior"))
                {
                    BaseBehavior attBehavior = ScriptableObject.CreateInstance<ArrowAttBehavior>();
                    attBehavior.Init(BehaviorPriority.Att, target.transform, 4.0f);

                    fsm.CheckAndAddBehavior(attBehavior);
                }
            }
            else
            {
                runBehaviorData.dest = target.transform.position;
                if (fsm.CurBehaviorType != Type.GetType("RunBehavior"))
                {
                    BaseBehavior walkBehavior = ScriptableObject.CreateInstance<RunBehavior>();
                    walkBehavior.Init(BehaviorPriority.Basic, runBehaviorData,0);
                    fsm.CheckAndAddBehavior(walkBehavior);
                }
            }
        }
    }
}
