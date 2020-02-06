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

    private BaseBehavior manualRunBehavior;
    private ManualRunBehaviorData manualRunData;
    private BaseBehavior attBehavior;
    private SwordAttBehaviorData attData;

    private string curAtt="none";
    private float curAttTime = 0;
    private const float att1Time = 0.15f;
    private const float att2Time = 0.15f;
    private const float att3Time = 1.0f;

    public override void Start()
    {
        base.Start();

        trail = GetComponentInChildren<WeaponTrail>();
        weapon = GetComponentInChildren<Sword>();

        manualRunData = new ManualRunBehaviorData(true, speed);
        manualRunBehavior = ScriptableObject.CreateInstance<ManualRunBehavior>();
        manualRunBehavior.Init(BehaviorPriority.Basic, manualRunData,true);
        attData = new SwordAttBehaviorData("att", 0);
        attBehavior = ScriptableObject.CreateInstance<SwordAttBehavior>();
        attBehavior.Init(BehaviorPriority.Skill, attData,false);
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

#if UNITY_EDITOR
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        Vector3 moveDir =
            Input.GetAxisRaw("Horizontal") * camRight +
            Input.GetAxisRaw("Vertical") * camForward;
        manualRunData.dir = moveDir.normalized;
        manualRunData.isRunning = (moveDir.magnitude > 0.0001f);

        // 보류
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (curAttTime < 0.01f)
            {
                switch (curAtt)
                {
                    case "none":
                        curAtt = "att1";
                        curAttTime = att1Time;
                        attData.lifeTime = curAttTime;
                        fsm.AddBehavior(attBehavior);
                        break;
                    case "att1":
                        curAtt = "att2";
                        curAttTime = att2Time;
                        attData.lifeTime = curAttTime;
                        fsm.AddBehavior(attBehavior);
                        break;
                    case "att2":
                        curAtt = "att3";
                        curAttTime = att3Time;
                        attData.lifeTime = att3Time;
                        fsm.AddBehavior(attBehavior);
                        break;
                    case "att3":
                        curAtt = "none";
                        break;
                }

            }
        }
#elif UNITY_ANDROID
#endif

        if (manualRunData.isRunning)
        {
            fsm.AddBehavior(manualRunBehavior);
        }
        else
        {
            fsm.AddBehavior(idleBehavior);
        }
    }

    private void OnDrawGizmos()
    {
    }
}
