using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[System.Serializable]
public struct PlayerSwordInfo
{
    public GameObject obj;
    public Transform skillPt;
}

public class Player : Mob
{

    [SerializeField] private float speed = 5.0f;
    [SerializeField] private Slider hpBar;
    [SerializeField] private Text hpAmount;

    [Header("Model")]
    public GameObject[] modelHairs;
    public GameObject[] modelCloths;
    public PlayerSwordInfo[] modelSwords;
    public GameObject[] modelShoulderPads;

    [Header("UI")]
    [SerializeField] private SkillButton sk1Button;
    [SerializeField] private SkillButton sk2Button;
    [SerializeField] private SkillButton sk3Button;
    [Header("Camera")]
    [SerializeField] private GameObject followCamera;
    [SerializeField] private Vector3 camPosOffset=new Vector3(0,5,-3);
    [SerializeField] private Vector3 camRotOffset=new Vector3(60,90,0);

    [Header("Effect & Prefab")]
    [SerializeField] private GameObject rollEffectPrefab;
    [SerializeField] private GameObject fireBallPrefab;
    [SerializeField] private GameObject golemSpawnBodyPrefab;
    [SerializeField] private GameObject golemSpawnFinishPrefab;
    [SerializeField] private GameObject golemBallPrefab;
    [SerializeField] private Golem golem = null;

    private RangeIndicator rIndicator=null;
    private GameObject golemSpawnBodyEffect;
    private WeaponTrail trail;
    private MeleeWeapon weapon;
    private Vector3 camFollowPt;
    private Transform fireBallPt;
    private NPC autoTarget=null;

    private float curTime = 0;
    private float curSpawnTime = 0;
    private float golemGrowRate = 0;
    private const float att1Damage = 45;
    private const float att2Damage = 50;
    private const float att3Damage = 65;
    private const float att1Time = 0.6f;
    private const float att2Time = 0.6f;
    private const float att3Time = 1.0f;
    private const float nextAttTime = 0.3f;
    private const float skBallTime = 0.6f;
    private const float rollTime = 0.5f;
    private const float rollSpeed = 13.0f;
    private Vector2 rIndicatorMaxRad = new Vector2(2.5f, 5.0f);
    private const float spawnChargeTime = 6.0f;
    private const float camZoonOutTime = 6.0f;
    private const float camMaxDist = 11.0f;

    private bool isSequenceAtt = false;

    private enum PlayerBehavior
    {
        Idle,
        Run,
        Roll,
        Att1,
        Att2,
        Att3,
        Sk_FireBall,
        Sk_SpawnGolem,
        Sk_JumpGolem,
        Die
    }
    PlayerBehavior curBehavior= PlayerBehavior.Idle;


    public override void GetDamaged(float amount)
    {
        base.GetDamaged(amount);


        // die this time
        if (curHP > 0 && curHP <= amount)
        {
            StartNewState(PlayerBehavior.Die);
        }

        curHP -= amount;
    }

    public override void GetMessage(Mob sender, MobMessage msg)
    {
        switch (msg)
        {
            case MobMessage.Die:
                if (autoTarget == sender)
                    autoTarget = null;
                break;
        }
    }

    public override void Start()
    {
        base.Start();

        SetupModels();

        rIndicator = GetComponentInChildren<RangeIndicator>();
        rIndicator.Init(rIndicatorMaxRad.x, Color.red, 360);
        rIndicator.gameObject.SetActive(false);

        followCamera.transform.Rotate(camRotOffset);
        camFollowPt = transform.position + camPosOffset;
        followCamera.transform.position = camFollowPt;

        sk2Button.Active(false);
        sk3Button.Active(false);
    }

    private void SetupModels()
    {
        if (PlayerPrefs.HasKey("Hair"))
        {
            for (int i = 0; i < modelHairs.Length; ++i)
            {
                if (PlayerPrefs.GetInt("Hair") == i)
                {
                    modelHairs[i].SetActive(true);
                }
                else
                {
                    Destroy(modelHairs[i]);
                }
                modelHairs[i] = null;
            }
        }
        else
        {
            modelHairs[0].SetActive(true);
        }
        if (PlayerPrefs.HasKey("Cloth"))
        {
            for (int i = 0; i < modelCloths.Length; ++i)
            {
                if (PlayerPrefs.GetInt("Cloth") == i)
                {
                    modelCloths[i].SetActive(true);
                }
                else
                {
                    Destroy(modelCloths[i]);
                }
                modelCloths[i] = null;
            }

        }
        else
        {
            modelCloths[0].SetActive(true);
        }
        if (PlayerPrefs.HasKey("Sword"))
        {
            int swordIdx = PlayerPrefs.GetInt("Sword");
            for (int i = 0; i < modelSwords.Length; ++i)
            {
                if (swordIdx == i)
                {
                    modelSwords[i].obj.SetActive(true);
                }
                else
                {
                    Destroy(modelSwords[i].obj);
                    modelSwords[i].obj = null;
                    modelSwords[i].skillPt = null;
                }

            }

            trail = modelSwords[swordIdx].obj.GetComponentInChildren<WeaponTrail>();
            weapon = modelSwords[swordIdx].obj.GetComponent<MeleeWeapon>();


            fireBallPt = modelSwords[swordIdx].skillPt;
        }
        else
        {
            trail = modelSwords[0].obj.GetComponentInChildren<WeaponTrail>();
            weapon = modelSwords[0].obj.GetComponent<MeleeWeapon>();

            fireBallPt = modelSwords[0].skillPt;
            modelSwords[0].obj.SetActive(true);
        }
        if (PlayerPrefs.HasKey("ShoulderPad"))
        {
            for (int i = 0; i < modelShoulderPads.Length; ++i)
            {
                if (PlayerPrefs.GetInt("ShoulderPad") == i)
                {
                    modelShoulderPads[i].SetActive(true);
                }
                else
                {
                    Destroy(modelShoulderPads[i]);
                }
                modelShoulderPads[i] = null;
            }
        }
        else
        {
            modelShoulderPads[0].SetActive(true);
        }
    }

    public void Init()
    {
        sk2Button.Active(true);
        sk3Button.Active(false);

        isUpdating = true;
    }

    public override void AE_StartAttack()
    {
        if (trail)
            trail.StartTrail();
        weapon.StartAttack();
    }
    public override void AE_EndAttack()
    {
        if (trail)
            trail.EndTrail();
        weapon.EndAttack();
    }
    public void AE_FireBall()
    {
        if(curBehavior==PlayerBehavior.Sk_FireBall)
        {
            Instantiate(fireBallPrefab, fireBallPt.position, transform.rotation);
        }
    }

    public void BT_Skill1()
    {
        if((curBehavior == PlayerBehavior.Idle || curBehavior == PlayerBehavior.Run) && sk1Button.IsReady)
        {
            StartNewState(PlayerBehavior.Sk_FireBall);
            sk1Button.StartCooldown();
        }
    }
    public void BT_Skill2()
    {
        if ((curBehavior == PlayerBehavior.Idle || curBehavior == PlayerBehavior.Run) && sk2Button.IsReady)
        {
            StartNewState(PlayerBehavior.Sk_SpawnGolem);
        }
    }
    public void BT_Skill2_2()
    {
        if(sk3Button.IsReady)
            StartNewState(PlayerBehavior.Sk_JumpGolem);
    }
    public void SpawnGolem(Vector3 pos)
    {
        golem.gameObject.SetActive(true);
        golem.transform.position=pos;
        golem.InitGolem(this, golemGrowRate);

        sk2Button.StartCooldown();
        sk2Button.Active(false);
    }
    public void ReadyToGolemJump()
    {
        sk3Button.Active(true);
    }
    public void DieGolem()
    {
        sk2Button.Active(true);
        sk3Button.Active(false);
    }

    private void StartNewState(PlayerBehavior behavior)
    {
        curTime = 0;
        rigid.velocity = Vector3.zero;
        rIndicator.gameObject.SetActive(false);


        trail.EndTrail();
        weapon.EndAttack();

        switch (behavior)
        {
            case PlayerBehavior.Idle:
                anim.SetTrigger("idle");
                break;
            case PlayerBehavior.Run:
                anim.SetTrigger("run");
                break;
            case PlayerBehavior.Roll:
                anim.SetTrigger("roll");
                Instantiate(rollEffectPrefab, transform);
                SoundMgr.Instance.Play(mainSoundPlayer, "Sliding", 1.0f);
                break;
            case PlayerBehavior.Att1:
                AutoTargetting();
                anim.SetTrigger("att1");
                weapon.SetDamage(att1Damage);
                PlayMainSound("PlayerAtt1",0.5f);
                isSequenceAtt = false;
                break;
            case PlayerBehavior.Att2:
                AutoTargetting();
                anim.SetTrigger("att2");
                PlayMainSound("PlayerAtt2",0.3f);
                weapon.SetDamage(att2Damage);
                isSequenceAtt = false;
                break;
            case PlayerBehavior.Att3:
                AutoTargetting();
                anim.SetTrigger("att3");
                PlayMainSound("PlayerAtt1",0.75f);
                weapon.SetDamage(att3Damage);
                isSequenceAtt = false;
                break;
            case PlayerBehavior.Sk_FireBall:
                anim.SetTrigger("sk_fireball");
                trail.SetColor(Color.red);
                SoundMgr.Instance.Play(mainSoundPlayer, "PlayerBallSwing", 1.0f);
                break;
            case PlayerBehavior.Sk_SpawnGolem:
                anim.SetTrigger("sk_spawnGolem");
                curSpawnTime = 0;
                rIndicator.SetPosition(transform.position);
                rIndicator.SetMaxRad(rIndicatorMaxRad.y);
                golemSpawnBodyEffect = Instantiate(golemSpawnBodyPrefab, transform.position, Quaternion.identity);
                break;
            case PlayerBehavior.Sk_JumpGolem:
                anim.SetTrigger("idle");
                rIndicator.gameObject.SetActive(true);
                rIndicator.SetMaxRad(4.0f);
                sk3Button.StartCooldown();
                break;
            case PlayerBehavior.Die:
                anim.SetTrigger("die");
                GetComponent<Collider>().enabled = false;
                break;
        }

        curBehavior = behavior;
    }

    private void AutoTargetting()
    {
        const float attRad = 1.5f;
        const float sqrAttRad = attRad* attRad;

        if (autoTarget) // target & close enough to melee attack
        {
            Vector3 subVec = autoTarget.transform.position - transform.position;
            subVec.y = 0;

            if (subVec.sqrMagnitude <= sqrAttRad)
            {
                transform.forward = subVec.normalized;
                return;
            }
        }

        Collider[] colls = Physics.OverlapSphere(transform.position, 5.0f, LayerMask.GetMask("Enemy"));
        float closestDist = float.MaxValue;
        autoTarget = null;
        foreach (var item in colls)
        {
            NPC curMob = item.GetComponent<NPC>();
            if (curMob)
            {
                Vector3 subVec = item.transform.position - transform.position;
                float sqrDist = subVec.sqrMagnitude;
                if (sqrDist < closestDist)
                {
                    closestDist = sqrDist;
                    autoTarget = curMob;
                }
            }
        }

        if (autoTarget)
        {
            Vector3 subVec = autoTarget.transform.position - transform.position;
            subVec.y = 0;
            if(subVec.sqrMagnitude > sqrAttRad)
                StartCoroutine(IE_AttackMoving());
            transform.forward = subVec.normalized;
        }
    }

    private IEnumerator IE_AttackMoving()
    {
        const float dist = 1.25f;
        const float time = 0.25f;
        float curTime = 0;
        float t = 0;
        while(t < 1)
        {
            t = curTime / time;
            Func<float, float> func = x => (1 + Mathf.Pow(x - 1, 3));
            float calculusHeightDiff = (func(t)-func(t-Time.deltaTime))*dist;

            transform.position += transform.forward * calculusHeightDiff;

            curTime += Time.deltaTime;

            yield return null;
        }
    }

    void Update()
    {
        if (IsDeath() || !isUpdating)
            return;

        hpBar.value = curHP / maxHP;
        hpAmount.text = curHP.ToString();

        curTime += Time.deltaTime;

        Vector3 newDir = transform.forward;
        bool isRunning = false;

#if UNITY_EDITOR
        isRunning =
            Input.GetKey(KeyCode.A)||
            Input.GetKey(KeyCode.D) ||
        Input.GetKey(KeyCode.W) ||
        Input.GetKey(KeyCode.S);

        if (isRunning)
        {
            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0;
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;
            Vector3 moveDir =
                Input.GetAxisRaw("Horizontal") * camRight +
                Input.GetAxisRaw("Vertical") * camForward;
            newDir = moveDir.normalized;
        }

#elif UNITY_ANDROID
        if (MobileTouch.Instance.IsOn)
        {
            if (MobileTouch.Instance.SqrFirstDragDist() > 6000)
            {
                isRunning = true;
                Vector3 dragDirScnPt = MobileTouch.Instance.DragDir();
                dragDirScnPt.z = dragDirScnPt.y;
                dragDirScnPt.y = 0;

                Vector3 camVerticalForward = Camera.main.transform.forward;
                camVerticalForward.y = 0;
                camVerticalForward.Normalize();
                Vector3 yAxis = Vector3.up;
                Vector3 zAxiz = camVerticalForward;
                Vector3 xAxis = Vector3.Cross(yAxis, zAxiz);
                Matrix4x4 mat = new Matrix4x4(
                    xAxis,
                    yAxis,
                    zAxiz,
                    new Vector4(0, 0, 0, 1)
                    );

                newDir = mat.MultiplyVector(dragDirScnPt);
            }
        }
#endif

        switch (curBehavior)
        {
            case PlayerBehavior.Idle:
#if UNITY_EDITOR
                if(Input.GetKeyDown(KeyCode.C))
                {
                    transform.LookAt(transform.position + newDir, Vector3.up);
                    StartNewState(PlayerBehavior.Roll);
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartNewState(PlayerBehavior.Att1);
                }
                else if (isRunning)
                {
                    StartNewState(PlayerBehavior.Run);
                }


#elif UNITY_ANDROID
                if (MobileTouch.Instance.IsOn)
                {
                    if (MobileTouch.Instance.GetTouchPhase == TouchPhase.Ended)
                    {
                        if (MobileTouch.Instance.SqrStationaryDragDist() > 50000.0f && MobileTouch.Instance.StationaryDragTime() < 0.3f)// roll
                        {
                            transform.LookAt(transform.position + newDir, Vector3.up);
                            StartNewState(PlayerBehavior.Roll);
                        }
                        else if(MobileTouch.Instance.SqrFirstDragDist() < 1000.0f)
                        {
                            StartNewState(PlayerBehavior.Att1);
                        }
                    }
                    else if (isRunning)
                    {
                        StartNewState(PlayerBehavior.Run);
                    }
                }
#endif
                break;
            case PlayerBehavior.Run:
                {
#if UNITY_EDITOR
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        transform.LookAt(transform.position + newDir, Vector3.up);
                        StartNewState(PlayerBehavior.Roll);
                    }
                    else if (Input.GetKeyDown(KeyCode.Space))
                    {
                        StartNewState(PlayerBehavior.Att1);
                    }
                    else if (!isRunning)
                    {
                        StartNewState(PlayerBehavior.Idle);
                    }
                    else
                    {
                        transform.position += newDir * speed * Time.deltaTime;

                        transform.LookAt(transform.position + newDir, Vector3.up);
                    }

#elif UNITY_ANDROID
                    if (MobileTouch.Instance.IsOn)
                    {
                        if (MobileTouch.Instance.GetTouchPhase == TouchPhase.Ended)
                        {
                            if (MobileTouch.Instance.SqrStationaryDragDist() > 50000.0f && MobileTouch.Instance.StationaryDragTime() < 0.3f)// roll
                            {
                                transform.LookAt(transform.position + newDir, Vector3.up);
                                StartNewState(PlayerBehavior.Roll);
                                break;
                            }
                            else if (MobileTouch.Instance.SqrFirstDragDist() < 1000.0f)
                            {
                                StartNewState(PlayerBehavior.Att1);
                                break;
                            }
                        }
                    }

                    if(!isRunning)
                    {
                        StartNewState(PlayerBehavior.Idle);
                    }
                    else
                    {
                        transform.position += newDir * speed * Time.deltaTime;

                        transform.LookAt(transform.position + newDir, Vector3.up);
                    }
#endif
                }
                break;

            case PlayerBehavior.Roll://-------------------------------------------------------------ROLL---------------------------------------
                {
                    float t = Mathf.Clamp01(curTime / rollTime);
                    float mt = Mathf.Log(Mathf.Sin(t * Mathf.PI), 4);
                    transform.position += transform.forward * rollSpeed * Time.deltaTime * t;

                    if (curTime >= rollTime)
                        StartNewState(isRunning ? PlayerBehavior.Run : PlayerBehavior.Idle);
                }
                break;
            case PlayerBehavior.Att1://-------------------------------------------------------------ATT1---------------------------------------
#if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.C))
                {
                    transform.LookAt(transform.position + newDir, Vector3.up);
                    StartNewState(PlayerBehavior.Roll);
                }
                else if(curTime >= att1Time)
                {
                    if(isSequenceAtt)
                    {
                        StartNewState(PlayerBehavior.Att2);
                    }
                    else
                    {
                        StartNewState(isRunning ? PlayerBehavior.Run : PlayerBehavior.Idle);
                    }
                }
                else if(Input.GetKeyDown(KeyCode.Space) && ((att1Time - curTime) < nextAttTime))
                {
                    isSequenceAtt = true;
                }

#elif UNITY_ANDROID
                if (MobileTouch.Instance.IsOn)
                {
                    if (MobileTouch.Instance.GetTouchPhase == TouchPhase.Ended)
                    {
                        if (MobileTouch.Instance.SqrStationaryDragDist() > 50000.0f && MobileTouch.Instance.StationaryDragTime() < 0.3f)// roll
                        {
                            transform.LookAt(transform.position + newDir, Vector3.up);
                            {
                                StartNewState(PlayerBehavior.Roll);
                                break;
                            }
                        }
                        else if (MobileTouch.Instance.SqrFirstDragDist() < 1000.0f && ((att1Time - curTime) < nextAttTime))
                        {
                            isSequenceAtt = true;
                        }
                    }
                }

                if(curTime>=att1Time)
                {
                    if (isSequenceAtt)
                    {
                        StartNewState(PlayerBehavior.Att2);
                    }
                    else
                    {
                        StartNewState(isRunning ? PlayerBehavior.Run : PlayerBehavior.Idle);
                    }
                }
#endif
                break;
            case PlayerBehavior.Att2://----------------------------------------------------------------------ATT2---------------------------------------
#if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.C))
                {
                    transform.LookAt(transform.position + newDir, Vector3.up);
                    StartNewState(PlayerBehavior.Roll);
                }
                else if (curTime >= att2Time)
                {
                    if(isSequenceAtt)
                    {
                        StartNewState(PlayerBehavior.Att3);
                    }
                    else
                    {
                        StartNewState(isRunning ? PlayerBehavior.Run : PlayerBehavior.Idle);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Space) && ((att2Time - curTime) < nextAttTime))
                {
                    isSequenceAtt = true;
                }
#elif UNITY_ANDROID

#endif
                break;
            case PlayerBehavior.Att3://-----------------------------------------------------------------------------------ATT3----------------------------
#if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.C))
                {
                    transform.LookAt(transform.position + newDir, Vector3.up);
                    StartNewState(PlayerBehavior.Roll);
                }
                else if (curTime >= att3Time)
                {
                    StartNewState(isRunning ? PlayerBehavior.Run : PlayerBehavior.Idle);
                }
#elif UNITY_ANDROID

#endif
                break;
            case PlayerBehavior.Sk_FireBall://------------------------------------------------------------------------SK1_FireBall---------------------------
                if (curTime >= skBallTime)
                {
                    StartNewState(isRunning ? PlayerBehavior.Run : PlayerBehavior.Idle);
                    trail.SetColor(Color.white);
                }
                    break;
            case PlayerBehavior.Sk_SpawnGolem://----------------------------------------------------------------------SK_GOLEM------------------------------

                if (Input.GetMouseButton(0))
                {
                    curSpawnTime += Time.deltaTime;
                    RaycastHit hit;
                    if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, LayerMask.GetMask("PlayGround")))
                    {
                        rIndicator.gameObject.SetActive(true);
                        float growRate = Mathf.Clamp01(curSpawnTime / spawnChargeTime);
                        float zoomT = Mathf.Clamp01(curSpawnTime / camZoonOutTime);
                        float curRIndicatorMaxRad = Mathf.Lerp(rIndicatorMaxRad.x, rIndicatorMaxRad.y, growRate);
                        rIndicator.SetProgress(growRate);
                        rIndicator.SetPosition(hit.point);

                        float camT = Mathf.Clamp01((1 - Mathf.Pow(zoomT - 1,2)) * 0.5f);
                        camFollowPt = transform.position + camPosOffset - followCamera.transform.forward * camMaxDist * camT;
                    }
                }
                else if(rIndicator.gameObject.activeSelf && Input.GetMouseButtonUp(0))
                {
                    rIndicator.gameObject.SetActive(false);
                    Destroy(golemSpawnBodyEffect);
                    Instantiate(golemSpawnFinishPrefab, transform.position, Quaternion.identity);
                    Vector3 ballDir = (new Vector3(1,-1,0)).normalized;
                    float growRate = Mathf.Clamp01(curSpawnTime / spawnChargeTime);
                    float ballDist = Mathf.Lerp(30,50, growRate);
                    golemGrowRate = growRate;
                    GolemBall golemBall = Instantiate(golemBallPrefab, rIndicator.transform.position - ballDir * ballDist, Quaternion.LookRotation(ballDir)).GetComponent<GolemBall>();
                    golemBall.Init(this, growRate, rIndicator.transform.position);

                    StartNewState(PlayerBehavior.Idle);
                }

                break;

            case PlayerBehavior.Sk_JumpGolem://-------------------------------------------------------------------JUMP_GOLEM---------------------------------
                {
                    if (golem)
                    {
                        float t = Mathf.Clamp01(curTime / camZoonOutTime);
                        float camT = (1 - Mathf.Pow(t - 1, 2)) * 0.5f;
                        camFollowPt = transform.position + camPosOffset - followCamera.transform.forward * camMaxDist * 0.5f * camT;

                        RaycastHit hit;
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, LayerMask.GetMask("PlayGround")))
                        {
                            rIndicator.SetPosition(hit.point);

                            if (Input.GetMouseButtonDown(0))
                            {
                                golem.Jump(hit.point);

                                StartNewState(PlayerBehavior.Idle);
                            }
                        }
                    }
                    else
                    {
                        StartNewState(PlayerBehavior.Idle);
                    }
                }
                break;

            case PlayerBehavior.Die:
                break;
        }

        if(curBehavior != PlayerBehavior.Sk_SpawnGolem && curBehavior != PlayerBehavior.Sk_JumpGolem)
            camFollowPt = transform.position + camPosOffset;
    }

    private void LateUpdate()
    {
        followCamera.transform.position = Vector3.Lerp(followCamera.transform.position, camFollowPt, 0.1f);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(transform.position, transform.position +)
    }
}

