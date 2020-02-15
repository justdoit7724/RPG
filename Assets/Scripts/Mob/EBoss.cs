using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorSelector
{
    Dictionary<Type, int> list = new Dictionary<Type, int>();
    Type[] selection = null;

    public void Add(string behaviorName, int preference)
    {
        preference=Mathf.Clamp(1, 10, preference);

        list.Add(Type.GetType(behaviorName), preference);

        Type[] tempArr = selection;
        if (tempArr != null)
        {
            selection = new Type[tempArr.Length + preference];
            for (int i = 0; i < tempArr.Length; ++i)
            {
                selection[i] = tempArr[i];
            }
            for (int i = tempArr.Length; i < selection.Length; ++i)
            {
                selection[i] = Type.GetType(behaviorName);
            }
        }
        else
        {
            selection = new Type[preference];
            for (int i = 0; i < preference; ++i)
            {
                selection[i] = Type.GetType(behaviorName);
            }
        }
    }

    public Type Get()
    {
        Type nextBehavior = null;
        try
        {
            nextBehavior = selection[UnityEngine.Random.Range(0, selection.Length)];
        }
        catch(System.IndexOutOfRangeException ex)
        {
            Debug.LogError("index out of range");
        }

        return nextBehavior;
    }
}

public class EBoss : NPC
{
    public float attRad = 1.3f;
    public float damage = 60;

    private MeleeWeapon wand;
    private BehaviorSelector selector;
    private WeaponTrail trail;
    private float colliderRad;
    private float sqrAttRad;

    private const float xMoveOffset= 15.0f;
    private const float zMoveOffset= 15.0f;


    public override void Start()
    {
        base.Start();

        sqrAttRad = attRad * attRad;

        wand = GetComponentInChildren<MeleeWeapon>();
        wand.SetDamage(damage);
        trail = GetComponentInChildren<WeaponTrail>();
        trail.SetColor(Color.blue);

        colliderRad = GetComponent<CapsuleCollider>().radius;

        selector = new BehaviorSelector();
        selector.Add("RunBehavior", 10);
        selector.Add("CloseAttBehavior", 4);
    }

    public override void AE_StartAttack()
    {
        trail.StartTrail();
        wand.enabled = true;
    }
    public override void AE_EndAttack()
    {
        trail.EndTrail();
        wand.enabled = false;
    }


    private void Update()
    {
        if (IsDeath())
            return;

        UpdateHPBar();

        if (fsm.Count == 0)
        {
            Type nextBehavior = selector.Get();
            switch(nextBehavior.ToString())
            {
                case "RunBehavior":
                    {
                        Vector3 rPos = transform.position;
                        for(int i=0; i<2; ++i)
                        {
                            rPos = transform.position + new Vector3(
                                UnityEngine.Random.Range(-1.0f,1.0f)*xMoveOffset,
                                0,
                                UnityEngine.Random.Range(-1.0f,1.0f)*zMoveOffset);

                            Collider[] colls = Physics.OverlapSphere(rPos, colliderRad, LayerMask.GetMask("Ground", "Wall"));
                            if(colls.Length==1 && colls[0].gameObject.layer == LayerMask.NameToLayer("Ground"))
                            {
                                runBehaviorData.dest = rPos;
                                BaseBehavior runBehavior = ScriptableObject.CreateInstance(nextBehavior) as BaseBehavior;
                                runBehavior.Init(BehaviorPriority.Basic, 0, runBehaviorData);
                                fsm.DirectAddBehavior(runBehavior);
                                BaseBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                                idleBehavior.Init(BehaviorPriority.Basic, 0.5f, null);
                                fsm.DirectAddBehavior(idleBehavior);
                                break;
                            }
                        }

                    }
                    break;
                case "CloseAttBehavior":
                    {
                        Collider[] colls = Physics.OverlapSphere(transform.position, attRad, LayerMask.GetMask("Alley"));
                        if(colls.Length>0)
                        {
                            Vector3 subVec = colls[0].transform.position - transform.position;
                            if(subVec.sqrMagnitude <= sqrAttRad)
                            {
                                transform.LookAt(colls[0].transform.position, Vector3.up);
                                BaseBehavior attBehavior = ScriptableObject.CreateInstance<CloseAttBehavior>() as BaseBehavior;
                                attBehavior.Init(BehaviorPriority.Att, 2.0f, null);
                                fsm.DirectAddBehavior(attBehavior);
                                IdleBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                                idleBehavior.Init(BehaviorPriority.Basic, 0.2f, null);
                                fsm.DirectAddBehavior(idleBehavior);
                            }
                        }
                    }
                    break;
            }
        }
    }


}

