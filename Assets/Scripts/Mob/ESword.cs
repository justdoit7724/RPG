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

        hpBar.UpdateBar(curHP, maxHP, transform.position);
        UpdateTarget("Alley", ref target);

        if (target == null)
        {
            if(!fsm.ContainBehavior(Type.GetType("IdleBehavior")))
            {
                BaseBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                idleBehavior.Init(BehaviorPriority.Basic, null, 0);
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

                    string animTrigger = "";
                    string soundKey = "";
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        animTrigger = "att1";
                        soundKey = "EnemyAtt";
                    }
                    else
                    {
                        animTrigger = "att2";
                        soundKey = "EnemyAttDouble";
                    }
                    BaseBehavior attBehavior = ScriptableObject.CreateInstance<AnimEventBehavior>();
                    attBehavior.Init(BehaviorPriority.Att, new AnimEventBData(animTrigger, mainSoundPlayer, soundKey, 0.2f), 2.0f);
                    fsm.CheckAndAddBehavior(attBehavior);
                }
            }
            else
            {
                runBehaviorData.dest = target.transform.position;
                if (!fsm.ContainBehavior(Type.GetType("RunBehavior")))
                {
                    BaseBehavior walkBehavior = ScriptableObject.CreateInstance<RunBehavior>();
                    walkBehavior.Init(BehaviorPriority.Basic, runBehaviorData,0);
                    fsm.CheckAndAddBehavior(walkBehavior);
                }
            }
        }
    }
}
