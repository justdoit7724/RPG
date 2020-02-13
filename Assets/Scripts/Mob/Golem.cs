using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : NPC
{
    [Header("Golem")]
    public Vector2 bodyScale = new Vector2(0.9f, 1.5f);
    public Vector2 attDamage = new Vector2(50, 90);
    public Vector2 stompDamage = new Vector2(120, 240);
    public Vector2 maxHPRange = new Vector2(700, 1200);
    public Vector2 attRad = new Vector2(1.5f, 2.0f);

    private MeleeWeapon fist;
    private float sqrAttRad;
    private float mStompDamage;

    public void Init(float growRate)
    {
        float mAttRad = Mathf.Lerp(attRad.x, attRad.y, growRate);
        sqrAttRad = mAttRad * mAttRad;
        float mScale = Mathf.Lerp(bodyScale.x, bodyScale.y, growRate);
        transform.localScale = new Vector3(mScale, mScale, mScale);
        mStompDamage = Mathf.Lerp(stompDamage.x, stompDamage.y, growRate);
        maxHP = Mathf.Lerp(maxHPRange.x, maxHPRange.y, growRate);

        fist = GetComponentInChildren<MeleeWeapon>();
        fist.SetDamage(Mathf.Lerp(attDamage.x, attDamage.y, growRate));

        enabled = false;

        StartCoroutine(IE_Enable());
    }

    private IEnumerator IE_Enable()
    {
        yield return new WaitForSeconds(1.5f);

        enabled = true;
    }
    public override void AE_StartAttack()
    {
        fist.enabled = true;
    }
    public override void AE_EndAttack()
    {
        fist.enabled = false;
    }

    private void Update()
    {
        if (IsDeath())
            return;

        UpdateHPBar();
        UpdateTarget("Enemy", ref target);

        if (target == null)
        {
            if (!fsm.ContainBehavior(Type.GetType("IdleBehavior")))
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

                    BaseBehavior att1Behavior = ScriptableObject.CreateInstance<CloseAttBehavior>();
                    att1Behavior.Init(BehaviorPriority.Skill, 4.0f, "att");
                    fsm.AddBehavior(att1Behavior);
                }
            }
            else if (!fsm.ContainBehavior(Type.GetType("RunBehavior")))
            {
                BaseBehavior walkBehavior = ScriptableObject.CreateInstance<RunBehavior>();
                walkBehavior.Init(BehaviorPriority.Basic, 1.0f, target.transform);
                fsm.AddBehavior(walkBehavior);
            }
        }
    }
}
