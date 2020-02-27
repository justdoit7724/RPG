using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossBehaviorKind
{
    Run = 0,
    MeleeAttack,
    Laser,
    Ball,
    Spawn,
    Curse
}

public class Selector
{
    Dictionary<BossBehaviorKind, int> list = new Dictionary<BossBehaviorKind, int>();
    BossBehaviorKind[] selection = null;

    public void Add(BossBehaviorKind behaviorName, int preference)
    {
        preference=Mathf.Clamp(preference, 1, 10);

        list.Add(behaviorName, preference);

        BossBehaviorKind[] tempArr = selection;
        if (tempArr != null)
        {
            selection = new BossBehaviorKind[tempArr.Length + preference];
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
            selection = new BossBehaviorKind[preference];
            for (int i = 0; i < preference; ++i)
            {
                selection[i] = behaviorName;
            }
        }
    }

    public BossBehaviorKind Get()
    {
        BossBehaviorKind nextBehavior = BossBehaviorKind.Run;
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
    public Material postprocessingMat;

    [Header("Prefab")]
    public GameObject laserPrefab;
    public Transform laserPt;
    public GameObject ballPrefab;
    public GameObject ballSpawnEffectPrefab;
    public GameObject mobSpawnEffectPrefab;
    public GameObject mobSpawnPtEffectPrefab;
    public GameObject swordMobPrefab;
    public GameObject bowMobPrefab;
    public GameObject rangeIndicatorPrefab;

    [Header("Info")]
    public float attRad = 2.5f;
    public float damage = 60;
    public float laserDamage = 10.0f;
    public float laserLength = 15.0f;

    private Lazer laser=null;
    private MeleeWeapon wand;
    private Selector selector;
    private WeaponTrail trail;
    private float colliderRad;
    private float sqrAttRad;
    private BossMovePt[] movePts;
    private MobSpawnPt[] mobSpawnPts;

    private const float laserDuration = 7.0f;
    private const float xMoveOffset= 20.0f;
    private const float zMoveOffset= 20.0f;

    private bool isCurseOn = false;


    private delegate void BossBehaviorFunc();
    private BossBehaviorFunc[] behaviorFunc = new BossBehaviorFunc[6];

    

    public override void Start()
    {
        base.Start();

        behaviorFunc[(int)BossBehaviorKind.Run] = Run;
        behaviorFunc[(int)BossBehaviorKind.MeleeAttack] = MeleeAttack;
        behaviorFunc[(int)BossBehaviorKind.Laser] = ShootLaser;
        behaviorFunc[(int)BossBehaviorKind.Ball] = FireBalls;
        behaviorFunc[(int)BossBehaviorKind.Spawn] = SpawnMobs;
        behaviorFunc[(int)BossBehaviorKind.Curse] = CursePlayer;

        sqrAttRad = attRad * attRad;

        wand = GetComponentInChildren<MeleeWeapon>();
        wand.SetDamage(damage);
        trail = GetComponentInChildren<WeaponTrail>();
        trail.SetColor(Color.blue);

        colliderRad = GetComponent<CapsuleCollider>().radius;

        mobSpawnPts = FindObjectsOfType<MobSpawnPt>();
        movePts = FindObjectsOfType<BossMovePt>();

        selector = new Selector();
        selector.Add(BossBehaviorKind.Run, /*9*/4);
        selector.Add(BossBehaviorKind.MeleeAttack, 10);
        selector.Add(BossBehaviorKind.Laser, 5);
        selector.Add(BossBehaviorKind.Ball, /*7*/3);
        selector.Add(BossBehaviorKind.Spawn, 3);
        selector.Add(BossBehaviorKind.Curse, 10);

        postprocessingMat.SetFloat("_Value", 0);
    }

    public override void AE_StartAttack()
    {
        trail.StartTrail();
        wand.StartAttack();
    }
    public override void AE_EndAttack()
    {
        trail.EndTrail();
        wand.EndAttack();
    }
    public void AE_StartLaser()
    {
        if (target)
        {
            laser = Instantiate(laserPrefab, laserPt).GetComponent<Lazer>();
            laser.Init(laserLength, laserDamage, 0.4f);

            LaserBehavior laserStayBehavior = ScriptableObject.CreateInstance<LaserBehavior>();
            laserStayBehavior.Init(BehaviorPriority.Skill, target, false, laserDuration);
            PlayMainSound("Laser", 0.3f);
            fsm.CheckAndAddBehavior(laserStayBehavior);
        }
        IdleBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
        idleBehavior.Init(BehaviorPriority.Basic, null, false, 2.0f);
        fsm.DirectAddBehavior(idleBehavior);

    }
    public void EndLaser()
    {
        if (laser)
        {
            laser.Destroy(2.0f);
            laser = null;
        }
    }
    public void AE_SpawnBall()
    {
        Instantiate(ballSpawnEffectPrefab, transform.position, Quaternion.identity);

        StartCoroutine(IE_Balls());
    }
    public void AE_StartSpawn()
    {
        Instantiate(mobSpawnEffectPrefab, transform.position, Quaternion.identity);

        StartCoroutine(IE_Spawn(2.0f));
    }

    private IEnumerator IE_Spawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        int spawnPtIdx = 0;
        NPC mob1 = null;
        NPC mob2 = null;
        for(int i=0; i<4; ++i)
        {
            Vector3 rPt = mobSpawnPts[spawnPtIdx++].transform.position;
            mob1 = Instantiate(swordMobPrefab, rPt, Quaternion.identity).GetComponent<NPC>();
            mob1.Init(0.75f);
            Instantiate(mobSpawnPtEffectPrefab, rPt, Quaternion.identity);


            yield return null;
        }
        
        mob1.PlayMainSound("BossSpawnMobAura1", 0.2f);
            
        for (int i = 0; i < 3; ++i)
        {
            Vector3 rPt = mobSpawnPts[spawnPtIdx++].transform.position;
            mob2 = Instantiate(bowMobPrefab, rPt, Quaternion.identity).GetComponent<NPC>();
            mob2.Init(0.75f);
            Instantiate(mobSpawnPtEffectPrefab, rPt, Quaternion.identity);


            yield return null;
        }

        mob2.PlayMainSound("BossSpawnMobAura2", 0.2f);
    }

    private IEnumerator IE_Balls()
    {
        yield return new WaitForSeconds(1.5f);

        const float height = 1.0f;
        const int ballNumPer = 12;
        float intvRad = (Mathf.PI*2.0f) / ballNumPer;
        float startRad = 0;
        const float volumeDownT =  1.0f/ (ballNumPer*2.0f);
        for (int i=0; i<ballNumPer; ++i)
        {
            mainSoundPlayer.volume -= volumeDownT;

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
            mainSoundPlayer.volume -= volumeDownT;

            Vector3 dir = new Vector3(
                Mathf.Cos(startRad+intvRad * i),
                0,
                Mathf.Sin(startRad+intvRad * i));

            Instantiate(ballPrefab, transform.position + dir * 0.1f + Vector3.up * height, Quaternion.LookRotation(dir, Vector3.up));

            yield return null;
        }
    }

    private void AE_Curse()
    {
        StartCoroutine(IE_CurseChange());
    }
    private IEnumerator IE_CurseChange()
    {
        if(!isCurseOn)
        {
            float t = 0;
            while(t<1)
            {
                t += 0.3f * Time.deltaTime;
                postprocessingMat.SetFloat("_Value", t);
                yield return null;
            }
        }
        else
        {
            float t = 1;
            while(t>0)
            {
                t -= 0.3f * Time.deltaTime;
                postprocessingMat.SetFloat("_Value", t);
                yield return null;
            }
        }


        isCurseOn = !isCurseOn;
    }

    private void ShuffleSpawnPts()
    {
        for(int i=0; i<5; ++i)
        {
            int rIdx1 = UnityEngine.Random.Range(0, mobSpawnPts.Length);
            int rIdx2 = UnityEngine.Random.Range(0, mobSpawnPts.Length);

            MobSpawnPt tempPt = mobSpawnPts[rIdx1];
            mobSpawnPts[rIdx1] = mobSpawnPts[rIdx2];
            mobSpawnPts[rIdx2] = tempPt;
        }
    }

    private void Update()
    {
        if (IsDeath() || !isUpdating)
            return;


        UpdateHPBar();

        if (fsm.Count == 0)
        {
            behaviorFunc[(int)selector.Get()]();
        }
    }

    private void Run()
    {
        runBehaviorData.dest = movePts[UnityEngine.Random.Range(0, movePts.Length)].transform.position;
        BaseBehavior runBehavior = ScriptableObject.CreateInstance<RunBehavior>();
        runBehavior.Init(BehaviorPriority.Basic, runBehaviorData, false, 5.0f);
        fsm.DirectAddBehavior(runBehavior);
        BaseBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
        idleBehavior.Init(BehaviorPriority.Basic, null, false, 2.0f);
        fsm.DirectAddBehavior(idleBehavior);
    }
    private void MeleeAttack()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, attRad, LayerMask.GetMask("Alley"));
        if (colls.Length > 0)
        {
            Vector3 subVec = colls[0].transform.position - transform.position;
            subVec.y = 0;
            if (subVec.sqrMagnitude <= sqrAttRad)
            {
                transform.forward = subVec.normalized;
                BaseBehavior attBehavior = ScriptableObject.CreateInstance<AnimEventBehavior>() as BaseBehavior;
                attBehavior.Init(BehaviorPriority.Att, "att", false, 2.0f);
                fsm.DirectAddBehavior(attBehavior);
                IdleBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                idleBehavior.Init(BehaviorPriority.Basic, null, false, 0.2f);
                fsm.DirectAddBehavior(idleBehavior);
            }
        }
    }
    private void ShootLaser()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, laserLength, LayerMask.GetMask("Alley"));
        float closestDist = float.MaxValue;
        target = null;
        foreach (var item in colls)
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

        if (target)
        {
            Vector3 subVec = target.transform.position - transform.position;
            subVec.y = 0;
            transform.forward = subVec.normalized;
            AnimEventBehavior laserStartBehavior = ScriptableObject.CreateInstance<AnimEventBehavior>();
            laserStartBehavior.Init(BehaviorPriority.Skill, "laser", true);
            fsm.DirectAddBehavior(laserStartBehavior);

            RangeIndicator rIndicator = Instantiate(rangeIndicatorPrefab, transform.position, Quaternion.identity).GetComponent<RangeIndicator>();
            rIndicator.Init(laserLength, Color.blue, 10);
            rIndicator.SetDir(transform.forward);
            rIndicator.StartProgress(1.5f, true);

            PlayMainSound("LaserCharge", 1.0f);
        }
    }
    private void FireBalls()
    {

        AnimEventBehavior animBehavior = ScriptableObject.CreateInstance<AnimEventBehavior>();
        animBehavior.Init(BehaviorPriority.Skill, "ball", false, 2.0f);
        fsm.DirectAddBehavior(animBehavior);
        IdleBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
        idleBehavior.Init(BehaviorPriority.Basic, null, false, 2.0f);
        fsm.DirectAddBehavior(idleBehavior);

        PlayMainSound("BossBallAura", 0.6f);
    }
    private void SpawnMobs()
    {
        AnimEventBehavior animBehavior = ScriptableObject.CreateInstance<AnimEventBehavior>();
        animBehavior.Init(BehaviorPriority.Skill, "spawn", false, 3.0f);
        fsm.DirectAddBehavior(animBehavior);
        IdleBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
        idleBehavior.Init(BehaviorPriority.Basic, null, false, 1.0f);
        fsm.DirectAddBehavior(idleBehavior);

        PlayMainSound("BossSpawnAura1");
        PlayMainSound("BossSpawnAura2");
    }
    private void CursePlayer()
    {
        if(target==null)
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, laserLength, LayerMask.GetMask("Alley"));
            float closestDist = float.MaxValue;
            foreach (var item in colls)
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
        }

        if (!isCurseOn && target)
        {
            CurseBehavior curseBehavior = ScriptableObject.CreateInstance<CurseBehavior>();
            int r1 = UnityEngine.Random.Range(0, movePts.Length);
            int r2 = UnityEngine.Random.Range(0, movePts.Length);
            while (r1 == r2)
            {
                r2 = UnityEngine.Random.Range(0, movePts.Length);
            }
            curseBehavior.Init(movePts[r1].transform.position, movePts[r2].transform.position, target, laserDuration, BehaviorPriority.Skill);
            fsm.DirectAddBehavior(curseBehavior);
        }
        else if(isCurseOn)
        {
            BaseBehavior cancelCurseBehavior = ScriptableObject.CreateInstance<AnimEventBehavior>();
            cancelCurseBehavior.Init(BehaviorPriority.Skill, "curse", false, 1.0f);
            fsm.DirectAddBehavior(cancelCurseBehavior);
        }
    }
}

