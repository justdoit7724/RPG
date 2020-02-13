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
        weapon.enabled = true;
    }
    public override void AE_EndAttack()
    {
        if (trail)
            trail.EndTrail();
        weapon.enabled = false;
    }

    // Update is called once per frame
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
                if (!fsm.ContainBehavior(Type.GetType("CloseAttBehavior")))
                {
                    transform.LookAt(target.transform.position, Vector3.up);

                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        BaseBehavior att1Behavior = ScriptableObject.CreateInstance<CloseAttBehavior>();
                        att1Behavior.Init(BehaviorPriority.Skill, 2.0f, "att1");
                        fsm.AddBehavior(att1Behavior);
                    }
                    else
                    {
                        BaseBehavior att2Behavior = ScriptableObject.CreateInstance<CloseAttBehavior>();
                        att2Behavior.Init(BehaviorPriority.Skill, 2.0f, "att2");
                        fsm.AddBehavior(att2Behavior);
                    }
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
