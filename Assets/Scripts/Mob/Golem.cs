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
    private float growRate;
    private CameraShakeSimpleScript camShaker;

    public void InitGolem(Player player, float growRate)
    {
        this.growRate = growRate;

        EBoss boss = FindObjectOfType<EBoss>();
        if(boss)
        {
            if(hpBar==null)
                hpBar = Instantiate(hpBarPrefab, uiCanvas).GetComponent<HPBar>();
            hpBar.ShowUp(!boss.IsCurseOn);
        }

        this.player = player;

        if(nav)
            nav.enabled = true;
        if (mainCollider)
            mainCollider.enabled = true;

        float mAttRad = Mathf.Lerp(attRad.x, attRad.y, growRate);
        sqrAttRad = mAttRad * mAttRad;
        float mScale = Mathf.Lerp(bodyScale.x, bodyScale.y, growRate);
        transform.localScale = new Vector3(mScale, mScale, mScale);
        mStompDamage = Mathf.Lerp(stompDamage.x, stompDamage.y, growRate);
        maxHP = Mathf.Lerp(maxHPRange.x, maxHPRange.y, growRate);
        curHP = maxHP;
        jumpSplash = Mathf.Lerp(jumpSplashRange.x, jumpSplashRange.y, growRate);
        GetComponent<CapsuleCollider>().radius = Mathf.Lerp(colliderSizeRange.x, colliderSizeRange.y, growRate);
        fist = GetComponentInChildren<MeleeWeapon>();
        fist.SetDamage(Mathf.Lerp(attDamage.x, attDamage.y, growRate));

        camShaker = Camera.main.GetComponent<CameraShakeSimpleScript>();
        camShaker.ShakeCaller(3.5f, 2.0f);

        StartCoroutine(IE_Enable());
    }

    public override void GetDamaged(float amount)
    {
        DamageEffect();

        // die this time
        if (curHP > 0 && curHP <= amount)
        {
            hpBar.transform.gameObject.SetActive(false);

            anim.SetTrigger("die");
            nav.enabled = false;
            mainCollider.enabled = false;
            StartCoroutine(IE_Disappear());
            StopUpdate();
            player.DieGolem();
            isUpdating = false;
            fsm.Clear();
            
            MobMgr.Instance.RemoveMob(this);
            MobMgr.Instance.SendMessage(this, MobMessage.Die);
        }

        curHP -= amount;
    }
    private IEnumerator IE_Disappear()
    {
        Vector3 curPos = transform.position;
        curPos.y = 0;
        transform.position = curPos;

        yield return new WaitForSeconds(3.0f);

        for(int i=0; i<300; ++i)
        {
            transform.position = transform.position + Vector3.down * 0.25f * Time.deltaTime;

            yield return null;
        }

        gameObject.SetActive(false);
    }

    private IEnumerator IE_Enable()
    {
        yield return new WaitForSeconds(1.5f);

        player.ReadyToGolemJump();

        isUpdating = true;
    }
    public override void AE_StartAttack()
    {
        fist.StartAttack();
    }
    public override void AE_EndAttack()
    {
        fist.EndAttack();
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
        float physicPower = Mathf.Lerp(300f, 450.0f, growRate);
        foreach (var item in colls)
        {
            NPC target = item.GetComponent<NPC>();
            if (target)
            {
                Vector3 subVec = target.transform.position - transform.position;
                float distRate = 1.0f - (subVec.magnitude / jumpSplash);
                float curDamage = distRate * mStompDamage;
                target.GetDamaged(curDamage);
                target.Rigid.AddForce(subVec.normalized * distRate * physicPower);
            }
        }

        camShaker.ShakeCaller(2.5f, 1.5f);
        SoundMgr.Instance.Play(mainSoundPlayer, "GolemHit", 1.0f);
    }
    public void AE_Step()
    {
        if (!mainSoundPlayer.isPlaying || mainSoundPlayer.clip.name == "GolemStep")
        {
            SoundMgr.Instance.Play(mainSoundPlayer, "GolemStep", 0.25f);
            camShaker.ShakeCaller(0.2f, 0.2f);
        }
    }
    public void Jump(Vector3 dest)
    {
        if (fsm.CurBehaviorType != Type.GetType("GolemJumpBehavior"))
        {
            target = null;

            GolemJumpBehavior jumpBehavior = ScriptableObject.CreateInstance<GolemJumpBehavior>();
            jumpBehavior.Init(dest, transform.position, BehaviorPriority.Skill);

            fsm.CheckAndAddBehavior(jumpBehavior);
        }
    }

    private void Update()
    {
        if (IsDeath() || !isUpdating)
            return;

        hpBar.UpdateBar(curHP, maxHP, transform.position);

        if (fsm.CurBehaviorType != Type.GetType("GolemJumpBehavior"))
        {
            UpdateTarget("Enemy", ref target);

            if (target == null)
            {
                if (fsm.CurBehaviorType != Type.GetType("IdleBehavior"))
                {
                    BaseBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                    idleBehavior.Init(BehaviorPriority.Basic, null, 0);
                    fsm.CheckAndAddBehavior(idleBehavior);
                }
            }
            else
            {
                Vector3 subVec = target.transform.position - transform.position;
                subVec.y = 0;
                if (subVec.sqrMagnitude <= sqrAttRad)
                {
                    if (fsm.CurBehaviorType != Type.GetType("AnimEventBehavior"))
                    {
                        transform.LookAt(target.transform.position, Vector3.up);

                        BaseBehavior att1Behavior = ScriptableObject.CreateInstance<AnimEventBehavior>();
                        att1Behavior.Init(BehaviorPriority.Att, new AnimEventBData("att"), 4.0f);
                        fsm.CheckAndAddBehavior(att1Behavior);
                    }
                }
                else
                {
                    runBehaviorData.dest = target.transform.position;
                    if (fsm.CurBehaviorType != Type.GetType("RunBehavior"))
                    {
                        BaseBehavior walkBehavior = ScriptableObject.CreateInstance<RunBehavior>();
                        walkBehavior.Init(BehaviorPriority.Basic, runBehaviorData, 1.0f);
                        fsm.CheckAndAddBehavior(walkBehavior);
                    }
                }
            }
        }
    }
}
