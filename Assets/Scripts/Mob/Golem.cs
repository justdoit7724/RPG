using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : NPC
{
    [Header("Golem")]
    public GameObject jumpHitEffectPrefab;
    public Vector2 bodyScale = new Vector2(0.9f, 1.5f);
    public Vector2 attDamage = new Vector2(50, 90);
    public Vector2 stompDamage = new Vector2(50, 130);
    public Vector2 maxHPRange = new Vector2(700, 1200);
    public Vector2 attRad = new Vector2(1.5f, 2.0f);
    public Vector2 jumpSplashRange = new Vector2(2.5f, 4.0f);
    public Vector2 colliderSizeRange = new Vector2(0.45f, 0.6f);

    private MeleeWeapon fist;
    private Player player;
    private float sqrAttRad;
    private float mStompDamage;
    private float jumpSplash;

    public void Init(float growRate)
    {
        float mAttRad = Mathf.Lerp(attRad.x, attRad.y, growRate);
        sqrAttRad = mAttRad * mAttRad;
        float mScale = Mathf.Lerp(bodyScale.x, bodyScale.y, growRate);
        transform.localScale = new Vector3(mScale, mScale, mScale);
        mStompDamage = Mathf.Lerp(stompDamage.x, stompDamage.y, growRate);
        maxHP = Mathf.Lerp(maxHPRange.x, maxHPRange.y, growRate);
        jumpSplash = Mathf.Lerp(jumpSplashRange.x, jumpSplashRange.y, growRate);
        GetComponent<CapsuleCollider>().radius = Mathf.Lerp(colliderSizeRange.x, colliderSizeRange.y, growRate);
        fist = GetComponentInChildren<MeleeWeapon>();
        fist.SetDamage(Mathf.Lerp(attDamage.x, attDamage.y, growRate));

        player = FindObjectOfType<Player>();
        if(player)
        {
            player.SpawnGolem(this);
        }

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
    public void AE_StartJump()
    {
        nav.enabled = false;
        mainCollider.enabled = false;
    }
    public void AE_EndJump()
    {
        nav.enabled = true;
        mainCollider.enabled = true;
        Instantiate(jumpHitEffectPrefab, transform.position, Quaternion.identity);

        Collider[] colls = Physics.OverlapSphere(transform.position, jumpSplash, LayerMask.GetMask("Enemy"));
        foreach (var item in colls)
        {
            NPC target = item.GetComponent<NPC>();
            if (target)
            {
                Vector3 subVec = target.transform.position - transform.position;
                float distRate = 1.0f - (subVec.magnitude / jumpSplash);
                float curDamage = distRate * mStompDamage;
                target.GetDamaged(curDamage);
                target.Rigid.AddForce(subVec.normalized * distRate * 150.0f);
            }
        }
    }
    public void Jump(Vector3 dest)
    {
        if (!fsm.ContainBehavior(Type.GetType("GolemJumpBehavior")))
        {
            GolemJumpBehavior jumpBehavior = ScriptableObject.CreateInstance<GolemJumpBehavior>();
            jumpBehavior.Init(dest, transform.position, BehaviorPriority.Skill);

            fsm.AddBehavior(jumpBehavior);
        }
    }

    public override void GetDamaged(float amount)
    {
        base.GetDamaged(amount);

        if(IsDeath())
        {
            player.DieGolem();
        }
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
                    att1Behavior.Init(BehaviorPriority.Att, 4.0f, "att");
                    fsm.AddBehavior(att1Behavior);
                }
            }
            else
            {
                runBehaviorData.dest = target.transform.position;
                if (!fsm.ContainBehavior(Type.GetType("RunBehavior")))
                {
                    BaseBehavior walkBehavior = ScriptableObject.CreateInstance<RunBehavior>();
                    walkBehavior.Init(BehaviorPriority.Basic, 1.0f, runBehaviorData);
                    fsm.AddBehavior(walkBehavior);
                }
            }
        }
    }
}
