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
        return selection[UnityEngine.Random.Range(0, selection.Length)];
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

    [Header("Info")]
    public float attRad = 2.5f;
    public float damage = 60;
    public float laserDamage = 10.0f;
    public float laserLength = 15.0f;

    [Header("Component")]
    public RangeIndicator rIndicator;
    public Transform wavePt;

    private Lazer laser = null;
    private MeleeWeapon wand;
    private Selector selector;
    private WeaponTrail trail;
    private float colliderRad;
    private float sqrAttRad;
    private BossMovePt[] movePts;
    private MobSpawnPt[] mobSpawnPts;
    private bool isCurseOn = false;
    private bool isCurseChanging = false;
    private float waveTime = 0;

    private const float laserDuration = 6.0f;
    private const float xMoveOffset = 20.0f;
    private const float zMoveOffset = 20.0f;
    private const float laserChargeTime = 1.7f;


    private delegate void BossBehaviorFunc();
    private BossBehaviorFunc[] behaviorFunc = new BossBehaviorFunc[6];

    public bool IsCurseOn{
        get {
        return isCurseOn;
        }
    }

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
        selector.Add(BossBehaviorKind.Run, 9);
        selector.Add(BossBehaviorKind.MeleeAttack, 10);
        selector.Add(BossBehaviorKind.Laser, 5);
        selector.Add(BossBehaviorKind.Ball, 9);
        selector.Add(BossBehaviorKind.Spawn, 4);
        selector.Add(BossBehaviorKind.Curse, 6);

        InitPostProcessing();

        rIndicator.Init(laserLength, Color.blue, 15);
    }

    private void InitPostProcessing()
    {
        postprocessingMat.SetFloat("_Value", 0.0f);
        postprocessingMat.SetFloat("_WaveValue", 0);
        postprocessingMat.SetVector("_WaveCenter", new Vector4(-1,-1,-1,-1));
    }

    public override void Die()
    {
        base.Die();
        DeleteLaser();
        postprocessingMat.SetVector("_WaveCenter", new Vector4(-1, -1, -1, -1));

        if(isCurseOn && !isCurseChanging)
            StartCoroutine(IE_CurseChange());
    }

    public void CreateLaser()
    {
        laser = Instantiate(laserPrefab, laserPt).GetComponent<Lazer>();
        laser.Init(laserLength, laserDamage, 0.35f);
    }
    public void DeleteLaser()
    {
        if (laser)
        {
            laser.Destroy(2.0f);
            laser = null;
        }
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

    public void AE_StartLaserRangeIndicator()
    {
        Vector3 subVec = target.transform.position - transform.position;
        subVec.y = 0;
        transform.forward = subVec.normalized;
        rIndicator.SetPosition(transform.position);
        rIndicator.SetDir(transform.forward);
        rIndicator.StartProgress(laserChargeTime, false);
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
        isCurseChanging = true;


        if (!isCurseOn)
        {
            MobMgr.Instance.SendMessage(this, MobMessage.CurseOn);
            hpBar.ShowUp(false);
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
            MobMgr.Instance.SendMessage(this, MobMessage.CurseOff);
            hpBar.ShowUp(true);
            float t = 1;
            while(t>0)
            {
                t -= 0.3f * Time.deltaTime;
                postprocessingMat.SetFloat("_Value", t);
                yield return null;
            }
        }

        isCurseChanging = false;
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

    private Vector4 World2UV()
    {
        Vector4 scnPos = Camera.main.WorldToScreenPoint(wavePt.position);

        scnPos.x /= Screen.width;
        scnPos.y /= Screen.height;
        return scnPos;
    }
    private void UpdateWave()
    {
        postprocessingMat.SetVector("_WaveCenter", World2UV());
    }
    private IEnumerator IE_Wave()
    {
        float curTime = 0;
        float t = 0;

        for (int i = 0; i < 5; ++i)
        {
            while (t < 1.0f)
            {
                curTime += Time.deltaTime;
                t = curTime / 0.5f;

                postprocessingMat.SetFloat("_WaveValue", t);

                yield return null;
            }

            t = 0;
            curTime = 0;
        }

        postprocessingMat.SetFloat("_WaveValue", 0);
    }

    private void Update()
    {
        if (IsDeath() || !isUpdating)
            return;

        UpdateWave();
        hpBar.UpdateBar(curHP, maxHP, transform.position);

        if (fsm.Count == 0)
        {
            behaviorFunc[(int)selector.Get()]();
        }
    }

    private void Run()
    {
        runBehaviorData.dest = movePts[UnityEngine.Random.Range(0, movePts.Length)].transform.position;
        BaseBehavior runBehavior = ScriptableObject.CreateInstance<RunBehavior>();
        runBehavior.Init(BehaviorPriority.Basic, runBehaviorData, 5.0f);
        fsm.DirectAddBehavior(runBehavior);
        BaseBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
        idleBehavior.Init(BehaviorPriority.Basic, null, 2.0f);
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
                attBehavior.Init(BehaviorPriority.Att, new AnimEventBData("att"), 2.0f);
                fsm.DirectAddBehavior(attBehavior);
                IdleBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
                idleBehavior.Init(BehaviorPriority.Basic, null, 0.2f);
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
            AnimEventBehavior laserStartBehavior = ScriptableObject.CreateInstance<AnimEventBehavior>();
            laserStartBehavior.Init(BehaviorPriority.Skill, new AnimEventBData("laser", mainSoundPlayer, "LaserCharge", 1.0f), laserChargeTime);
            fsm.DirectAddBehavior(laserStartBehavior);
           
            LaserBehavior laserStayBehavior = ScriptableObject.CreateInstance<LaserBehavior>();
            laserStayBehavior.Init(BehaviorPriority.Skill, new LaserBData(target, mainSoundPlayer, "Laser", 0.25f), laserDuration);
            fsm.DirectAddBehavior(laserStayBehavior);
            IdleBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
            idleBehavior.Init(BehaviorPriority.Basic, null, 2.5f);
            fsm.DirectAddBehavior(idleBehavior);
        }
    }
    private void FireBalls()
    {
        BaseBehavior animBehavior = ScriptableObject.CreateInstance<AnimEventBehavior>();
        animBehavior.Init(BehaviorPriority.Skill, new AnimEventBData("ball", mainSoundPlayer, "BossBallAura", 0.5f), 2.0f);
        fsm.DirectAddBehavior(animBehavior);
        IdleBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
        idleBehavior.Init(BehaviorPriority.Basic, null, 2.0f);
        fsm.DirectAddBehavior(idleBehavior);
    }
    private void SpawnMobs()
    {
        AnimEventBehavior animBehavior = ScriptableObject.CreateInstance<AnimEventBehavior>();
        animBehavior.Init(BehaviorPriority.Skill, new AnimEventBData("spawn", mainSoundPlayer, "BossSpawnAura2", 1.0f), 3.0f);
        fsm.DirectAddBehavior(animBehavior);
        IdleBehavior idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
        idleBehavior.Init(BehaviorPriority.Basic, null, 1.0f);
        fsm.DirectAddBehavior(idleBehavior);
    }
    private void CursePlayer()
    {
        if (isCurseChanging)
            return;

        if (target==null)
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

        if (target)
        {
            CurseBehavior curseBehavior = ScriptableObject.CreateInstance<CurseBehavior>();
            int r1 = UnityEngine.Random.Range(0, movePts.Length);
            int r2 = UnityEngine.Random.Range(0, movePts.Length);
            while (r1 == r2)
            {
                r2 = UnityEngine.Random.Range(0, movePts.Length);
            }
            curseBehavior.Init(laserPrefab, laserPt, laserLength, laserDamage, movePts[r1].transform.position, movePts[r2].transform.position,mainSoundPlayer, target, laserDuration, laserChargeTime, BehaviorPriority.Skill);
            fsm.DirectAddBehavior(curseBehavior);

            StartCoroutine(IE_Wave());
        }
    }
}

