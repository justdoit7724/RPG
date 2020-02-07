using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : Mob
{
    [SerializeField] private float speed=5.0f;
    [SerializeField] private Slider hpBar;
    [SerializeField] private Text hpAmount;

    private WeaponTrail trail;
    private Sword weapon;

    private SwordAttBehaviorData att1Data;
    private SwordAttBehaviorData att2Data;
    private SwordAttBehaviorData att3Data;

    private string curAtt="none";
    private float curAttTime = 0;

    public override void Start()
    {
        base.Start();

        trail = GetComponentInChildren<WeaponTrail>();
        weapon = GetComponentInChildren<Sword>();

        att1Data = new SwordAttBehaviorData("att1", 3.0f);
        att2Data = new SwordAttBehaviorData("att2", 3.0f);
        att3Data = new SwordAttBehaviorData("att3", 6.0f);
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
    void Update()
    {
        hpBar.value = curHP / maxHP;
        hpAmount.text = curHP.ToString();


        curAttTime -= Time.deltaTime;

        bool isRun = false;
#if UNITY_EDITOR
        isRun = 
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.S);

        // 보류
        if (Input.GetKeyDown(KeyCode.Space) && fsm.CanAdd(BehaviorPriority.Skill))
        {
            if (curAttTime <= 0)
            {
                curAtt = "att1";
                curAttTime = att1Data.lifeTime;

                BaseBehavior attBehavior = ScriptableObject.CreateInstance<SwordAttBehavior>();
                attBehavior.Init(BehaviorPriority.Skill, att1Data, false);
                fsm.AddBehavior(attBehavior);
            }
            else
            {
                switch (curAtt)
                {
                    case "att1":
                        if (curAttTime < 1.0f)
                        {
                            curAtt = "att2";
                            curAttTime = att2Data.lifeTime;

                            BaseBehavior attBehavior = ScriptableObject.CreateInstance<SwordAttBehavior>();
                            attBehavior.Init(BehaviorPriority.Skill, att2Data, false);
                            fsm.AddBehavior(attBehavior);
                        }
                        break;
                    case "att2":
                        if (curAttTime < 1.0f)
                        {
                            curAtt = "att3";
                            curAttTime = att3Data.lifeTime;

                            BaseBehavior attBehavior = ScriptableObject.CreateInstance<SwordAttBehavior>();
                            attBehavior.Init(BehaviorPriority.Skill, att3Data, false);
                            fsm.AddBehavior(attBehavior);
                        }
                        break;
                }
            }
        }
#elif UNITY_ANDROID
#endif
        if (fsm.CanAdd(BehaviorPriority.Basic))
        {
            if (isRun && fsm.CurBehaviorName() != "ManualRunBehavior")
            {
                BaseBehavior manualRunBehavior = ScriptableObject.CreateInstance<ManualRunBehavior>();
                manualRunBehavior.Init(BehaviorPriority.Basic, speed, true);
                fsm.AddBehavior(manualRunBehavior);
            }
            else if(fsm.CurBehaviorName() != "IdleBehavior")
            {
                BaseBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                idleBehavior.Init(BehaviorPriority.Basic, null, false);
                fsm.AddBehavior(idleBehavior);
            }
        }
    }

    private void OnDrawGizmos()
    {
    }
}
