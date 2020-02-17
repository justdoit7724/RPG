using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector
{
    Dictionary<string, int> list = new Dictionary<string, int>();
    string[] selection = null;

    public void Add(string behaviorName, int preference)
    {
        preference=Mathf.Clamp(1, 10, preference);

        list.Add(behaviorName, preference);

        string[] tempArr = selection;
        if (tempArr != null)
        {
            selection = new string[tempArr.Length + preference];
            for (int i = 0; i < tempArr.Length; ++i)
            {
                selection[i] = tempArr[i];
            }
            for (int i = tempArr.Length; i < selection.Length; ++i)
            {
                selection[i] = behaviorName;
            }
        }
        else
        {
            selection = new string[preference];
            for (int i = 0; i < preference; ++i)
            {
                selection[i] = behaviorName;
            }
        }
    }

    public string Get()
    {
        string nextBehavior = null;
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
    public GameObject laserPrefab;
    public Transform laserPt;
    public GameObject ballPrefab;
    public GameObject ballSpawnEffectPrefab;
    public GameObject mobSpawnEffectPrefab;
    public GameObject swordMobPrefab;
    public GameObject bowMobPrefab;
    public GameObject rangeIndicatorPrefab;

    public float attRad = 1.3f;
    public float damage = 60;
    public float laserDamage = 10.0f;
    public float laserLength = 15.0f;

    private Lazer laser=null;
    private MeleeWeapon wand;
    private Selector selector;
    private WeaponTrail trail;
    private float colliderRad;
    private float sqrAttRad;

    private const float laserDuration = 8.0f;
    private const float xMoveOffset= 20.0f;
    private const float zMoveOffset= 20.0f;


    public override void Start()
    {
        base.Start();

        sqrAttRad = attRad * attRad;

        wand = GetComponentInChildren<MeleeWeapon>();
        wand.SetDamage(damage);
        trail = GetComponentInChildren<WeaponTrail>();
        trail.SetColor(Color.blue);

        colliderRad = GetComponent<CapsuleCollider>().radius;

        selector = new Selector();
        selector.Add("RunBehavior", 3);
        selector.Add("CloseAtt", 9);
        selector.Add("LaserBehavior", 2);
        selector.Add("BallBehavior", 10);
        selector.Add("SpawnBehavior", 2);
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
    public void AE_StartLaser()
    {
        laser=Instantiate(laserPrefab, laserPt).GetComponent<Lazer>();
        laser.Init(laserLength, laserDamage, 0.4f);
    }
    public void EndLaser()
    {
        laser.GetComponent<Animation>().Play();
        Destroy(laser.gameObject, 2.0f);
        laser = null;
    }
    public void AE_SpawnBall()
    {
        Instantiate(ballSpawnEffectPrefab, transform.position, Quaternion.identity);

        StartCoroutine(IE_Balls());
    }
    public void AE_StartSpawn()
    {

    }

    private IEnumerator IE_Balls()
    {
        yield return new WaitForSeconds(1.0f);

        const float height = 1.0f;
        const int ballNumPer = 12;
        float intvRad = (Mathf.PI*2.0f) / ballNumPer;
        float startRad = 0;
        for(int i=0; i<ballNumPer; ++i)
        {
            Vector3 dir = new Vector3(
                Mathf.Cos(startRad+intvRad * i),
                0,
                Mathf.Sin(startRad+intvRad * i));

            Instantiate(ballPrefab, transform.position + dir * 0.1f + Vector3.up * height, Quaternion.LookRotation(dir, Vector3.up));

            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        startRad += Mathf.PI / 12.0f;
        for (int i = 0; i < ballNumPer; ++i)
        {
            Vector3 dir = new Vector3(
                Mathf.Cos(startRad+intvRad * i),
                0,
                Mathf.Sin(startRad+intvRad * i));

            Instantiate(ballPrefab, transform.position + dir * 0.1f + Vector3.up * height, Quaternion.LookRotation(dir, Vector3.up));

            yield return null;
        }
    }

    Vector3 tempDest;
    private void Update()
    {
        if (IsDeath())
            return;

        UpdateHPBar();

        if (fsm.Count == 0)
        {
            switch(selector.Get())
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
                                tempDest = rPos;
                                runBehaviorData.dest = rPos;
                                BaseBehavior runBehavior = ScriptableObject.CreateInstance<RunBehavior>();
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
                case "CloseAtt":
                    {
                        Collider[] colls = Physics.OverlapSphere(transform.position, attRad, LayerMask.GetMask("Alley"));
                        if(colls.Length>0)
                        {
                            Vector3 subVec = colls[0].transform.position - transform.position;
                            if(subVec.sqrMagnitude <= sqrAttRad)
                            {
                                transform.LookAt(colls[0].transform.position, Vector3.up);
                                BaseBehavior attBehavior = ScriptableObject.CreateInstance<AnimEventBehavior>() as BaseBehavior;
                                attBehavior.Init(BehaviorPriority.Att, 2.0f, "att");
                                fsm.DirectAddBehavior(attBehavior);
                                IdleBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                                idleBehavior.Init(BehaviorPriority.Basic, 0.2f, null);
                                fsm.DirectAddBehavior(idleBehavior);
                            }
                        }
                    }
                    break;
                case "LaserBehavior":
                    {
                        Collider[] colls = Physics.OverlapSphere(transform.position, laserLength, LayerMask.GetMask("Alley"));
                        float closestDist = float.MaxValue;
                        Mob target = null;
                        foreach(var item in colls)
                        {
                            Mob itemMob = item.GetComponent<Mob>();
                            if (itemMob)
                            {
                                Vector3 subVec = item.transform.position - transform.position;
                                float sqrDist = subVec.sqrMagnitude;
                                if (sqrDist <= closestDist)
                                {
                                    closestDist = sqrDist;
                                    target = itemMob;
                                }
                            }
                        }

                        if(target)
                        {
                            transform.LookAt(target.transform.position, Vector3.up);
                            LaserBehavior laserBehavior = ScriptableObject.CreateInstance<LaserBehavior>();
                            laserBehavior.Init(BehaviorPriority.Skill, 8, target.transform);
                            fsm.DirectAddBehavior(laserBehavior);
                            IdleBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                            idleBehavior.Init(BehaviorPriority.Basic, 2.0f, null);
                            fsm.DirectAddBehavior(idleBehavior);

                            RangeIndicator rIndicator = Instantiate(rangeIndicatorPrefab, transform.position, Quaternion.identity).GetComponent<RangeIndicator>();
                            rIndicator.Init(laserLength, Color.blue, 10);
                            rIndicator.SetDir(transform.forward);
                            rIndicator.StartProgress(1.0f, true);
                        }
                    }
                    break;
                case "BallBehavior":
                    {
                        AnimEventBehavior animBehavior = ScriptableObject.CreateInstance<AnimEventBehavior>();
                        animBehavior.Init(BehaviorPriority.Skill, 2.0f, "ball");
                        fsm.DirectAddBehavior(animBehavior);
                        IdleBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                        idleBehavior.Init(BehaviorPriority.Basic, 2.0f, null);
                        fsm.DirectAddBehavior(idleBehavior);
                    }

                    break;
                case "SpawnBehavior":
                    {
                        AnimEventBehavior animBehavior = ScriptableObject.CreateInstance<AnimEventBehavior>();
                        animBehavior.Init(BehaviorPriority.Skill, 2.0f, "spawn");
                        fsm.DirectAddBehavior(animBehavior);
                        IdleBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                        idleBehavior.Init(BehaviorPriority.Basic, 2.0f, null);
                        fsm.DirectAddBehavior(idleBehavior);
                    }
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.DrawWireSphere(tempDest, 0.5f);
    }
}

