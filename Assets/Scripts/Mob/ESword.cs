using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ESword : NPC
{
    [SerializeField] protected float attRad = 3;
    [SerializeField] private float swordDamage = 20;
    protected float sqrAttRad;
    private WeaponTrail trail;
    private MeleeWeapon weapon;

    public Mob Target { get { return target; } }


    public override void Start()
    {
        base.Start();

        sqrAttRad = attRad * attRad;

        fsm = GetComponent<FSM>();
        trail = GetComponentInChildren<WeaponTrail>();
        weapon = GetComponentInChildren<MeleeWeapon>();
        weapon.SetDamage(swordDamage);
    }
    public override void AE_StartAttack()
    {
        if (trail)
            trail.StartTrail();
        weapon.StartAttack();
    }
    public override void AE_EndAttack()
    {
        if (trail)
            trail.EndTrail();
        weapon.EndAttack();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDeath() || !isUpdating)
            return;

        UpdateHPBar();
        UpdateTarget("Alley", ref target);

        if (target == null)
        {
            if(!fsm.ContainBehavior(Type.GetType("IdleBehavior")))
            {
                BaseBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                idleBehavior.Init(BehaviorPriority.Basic, null, true);
                fsm.CheckAndAddBehavior(idleBehavior);
            }
        }
        else
        {
            Vector3 subVec = target.transform.position-transform.position;
            subVec.y = 0;
            if (subVec.sqrMagnitude <= sqrAttRad)
            {
                if (!fsm.ContainBehavior(Type.GetType("AnimEventBehavior")))
                {
                    transform.forward = subVec.normalized;

                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        BaseBehavior att1Behavior = ScriptableObject.CreateInstance<AnimEventBehavior>();
                        att1Behavior.Init(BehaviorPriority.Att, "att1", false, 2.0f);
                        fsm.CheckAndAddBehavior(att1Behavior);
                        PlayMainSound("EnemyAtt",0.5f);

                    }
                    else
                    {
                        BaseBehavior att2Behavior = ScriptableObject.CreateInstance<AnimEventBehavior>();
                        att2Behavior.Init(BehaviorPriority.Att, "att2", false, 2.0f);
                        fsm.CheckAndAddBehavior(att2Behavior);
                        PlayMainSound("EnemyAttDouble",0.2f);
                    }
                }
            }
            else
            {
                runBehaviorData.dest = target.transform.position;
                if (!fsm.ContainBehavior(Type.GetType("RunBehavior")))
                {
                    BaseBehavior walkBehavior = ScriptableObject.CreateInstance<RunBehavior>();
                    walkBehavior.Init(BehaviorPriority.Basic, runBehaviorData,true);
                    fsm.CheckAndAddBehavior(walkBehavior);
                }
            }
        }
    }
}
