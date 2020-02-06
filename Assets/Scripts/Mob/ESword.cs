﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ESword : Mob
{
    [SerializeField] private float detectRad = 20;
    [SerializeField] private float attRad = 2;
    private float sqrAttRad;
    private Mob target = null;
    private WeaponTrail trail;
    private Sword weapon;
    private BaseBehavior att1Behavior;
    private BaseBehavior att2Behavior;

    public Mob Target { get { return target; } }

    public override void Start()
    {
        base.Start();

        sqrAttRad = attRad * attRad;

        trail = GetComponentInChildren<WeaponTrail>();
        weapon = GetComponentInChildren<Sword>();

        att1Behavior = ScriptableObject.CreateInstance<SwordAttBehavior>();
        att2Behavior = ScriptableObject.CreateInstance<SwordAttBehavior>();
        att1Behavior.Init(BehaviorPriority.Skill, new SwordAttBehaviorData("att1", 1.0f),true);
        att2Behavior.Init(BehaviorPriority.Skill, new SwordAttBehaviorData("att2", 1.7f),true);
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
        if (target == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectRad, LayerMask.GetMask("Player"));
            if (colliders.Length > 0)
                target = colliders[0].GetComponent<Mob>();
            else
                fsm.AddBehavior(idleBehavior);
        }
        

        if(target)
        {
            Vector3 subVec = transform.position - target.transform.position;
            if (subVec.sqrMagnitude <= sqrAttRad)
            {
                fsm.AddBehavior((Random.Range(0, 2)==0) ? att1Behavior : att2Behavior);
            }
            else
            {
                walkData.dest = target.transform.position;
                fsm.AddBehavior(walkBehavior);
            }
        }
    }

    public void ClearTarget()
    {
        target = null;
    }

    private void OnDrawGizmos()
    {
       
    }
}
