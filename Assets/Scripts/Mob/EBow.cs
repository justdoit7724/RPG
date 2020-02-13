using System;
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
    }
    public override void AE_EndAttack()
    {
        visualArrow.SetActive(true);
    }

    

    void Update()
    {
        if (IsDeath())
            return;

        UpdateHPBar();
        UpdateTarget("Alley", ref target);

        if (target == null)
        {
            if(!fsm.ContainBehavior(Type.GetType("IdleBehavior")))
            {
                BaseBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                idleBehavior.Init(BehaviorPriority.Basic, 0, null);
                fsm.AddBehavior(idleBehavior);
            }
        }
        else
        {
            Vector3 subVec = transform.position - target.transform.position;
            if (subVec.sqrMagnitude <= sqrAttRad)
            {
                if (!fsm.ContainBehavior(Type.GetType("ArrowAttBehavior")))
                {
                    BaseBehavior attBehavior = ScriptableObject.CreateInstance<ArrowAttBehavior>();
                    attBehavior.Init(BehaviorPriority.Skill, 4.0f, target.transform);

                    fsm.AddBehavior(attBehavior);
                }
            }
            else if(!fsm.ContainBehavior(Type.GetType("RunBehavior")))
            {
                BaseBehavior walkBehavior = ScriptableObject.CreateInstance<RunBehavior>();
                runBehaviorData.dest = target.transform.position;
                walkBehavior.Init(BehaviorPriority.Basic, 0, runBehaviorData);
                fsm.AddBehavior(walkBehavior);
            }
        }
    }
}
