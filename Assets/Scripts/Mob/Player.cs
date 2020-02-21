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
    [SerializeField] private SkillButton fireBallSkButton;
    [SerializeField] private SkillButton golemSkill1Button;
    [SerializeField] private SkillButton golemSkill2Button;
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


    private RangeIndicator rIndicator=null;
    private GameObject golemSpawnBodyEffect;
    private WeaponTrail trail;
    private MeleeWeapon weapon;
    private Vector3 camFollowPt;
    private Golem golem = null;
    private Transform fireBallPt;

    private float curTime = 0;
    private float curSpawnTime = 0;
    private const float att1Damage = 45;
    private const float att2Damage = 50;
    private const float att3Damage = 65;
    private const float att1Time = 0.4f;
    private const float att2Time = 0.4f;
    private const float att3Time = 1.0f;
    private const float skBallTime = 0.6f;
    private const float rollTime = 0.5f;
    private const float rollSpeed = 13.0f;
    private Vector2 rIndicatorMaxRad = new Vector2(2.5f, 5.0f);
    private const float spawnChargeTime = 6.0f;
    private const float camZoonOutTime = 6.0f;
    private const float camMaxDist = 11.0f;

    private bool isSequenceAtt = false;
    private bool isRunning = false;

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

        golemSkill1Button.gameObject.SetActive(false);
        golemSkill2Button.gameObject.SetActive(false);

        enabled = false;
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
        golemSkill1Button.gameObject.SetActive(true);

        enabled = true;
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
        if((curBehavior == PlayerBehavior.Idle || curBehavior == PlayerBehavior.Run) && fireBallSkButton.IsReady)
        {
            StartNewState(PlayerBehavior.Sk_FireBall);
            fireBallSkButton.StartCooldown();
        }
    }
    public void BT_Skill2()
    {
        if ((curBehavior == PlayerBehavior.Idle || curBehavior == PlayerBehavior.Run) && golemSkill1Button.IsReady)
        {
            StartNewState(PlayerBehavior.Sk_SpawnGolem);
        }
    }
    public void BT_Skill2_2()
    {
        if(golemSkill2Button.IsReady)
            StartNewState(PlayerBehavior.Sk_JumpGolem);
    }
    public void SpawnGolem(Golem golem)
    {
        this.golem = golem;

        golemSkill1Button.gameObject.SetActive(false);
        golemSkill2Button.gameObject.SetActive(true);
    }
    public void DieGolem()
    {
        golemSkill1Button.StartCooldown();
        golem = null;

        golemSkill1Button.gameObject.SetActive(true);
        golemSkill2Button.gameObject.SetActive(false);
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
                break;
            case PlayerBehavior.Att1:
                anim.SetTrigger("att1");
                weapon.SetDamage(att1Damage);
                isSequenceAtt = false;
                break;
            case PlayerBehavior.Att2:
                anim.SetTrigger("att2");
                weapon.SetDamage(att2Damage);
                isSequenceAtt = false;
                break;
            case PlayerBehavior.Att3:
                anim.SetTrigger("att3");
                weapon.SetDamage(att3Damage);
                isSequenceAtt = false;
                break;
            case PlayerBehavior.Sk_FireBall:
                anim.SetTrigger("sk_fireball");
                trail.SetColor(Color.red);
                break;
            case PlayerBehavior.Sk_SpawnGolem:
                anim.SetTrigger("sk_spawnGolem");
                curSpawnTime = 0;
                rIndicator.SetPosition(transform.position);
                rIndicator.SetMaxRad(rIndicatorMaxRad.y);
                golemSpawnBodyEffect = Instantiate(golemSpawnBodyPrefab, transform.position, Quaternion.identity);
                golemSkill1Button.StartCooldown();
                golemSkill2Button.StartCooldown();
                break;
            case PlayerBehavior.Sk_JumpGolem:
                anim.SetTrigger("idle");
                rIndicator.gameObject.SetActive(true);
                rIndicator.SetMaxRad(4.0f);
                golemSkill2Button.StartCooldown();
                break;
            case PlayerBehavior.Die:
                anim.SetTrigger("die");
                GetComponent<Collider>().enabled = false;
                break;
        }

        curBehavior = behavior;
    }

    void Update()
    {
        hpBar.value = curHP / maxHP;
        hpAmount.text = curHP.ToString();

        curTime += Time.deltaTime;

#if UNITY_EDITOR
        isRunning = Input.GetKey(KeyCode.A) ||
                    Input.GetKey(KeyCode.D) ||
                    Input.GetKey(KeyCode.W) ||
                    Input.GetKey(KeyCode.S);

        Vector3 newDir = transform.forward;
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
#endif
                break;
            case PlayerBehavior.Run:

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
                else if(Input.GetKeyDown(KeyCode.Space) && (curTime > 0.2f))
                {
                    isSequenceAtt = true;
                }
#elif UNITY_ANDROID

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
                else if (Input.GetKeyDown(KeyCode.Space) && (curTime > 0.2f))
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
                    if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, LayerMask.GetMask("Ground")))
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
                    GolemBall golemBall = Instantiate(golemBallPrefab, rIndicator.transform.position - ballDir * ballDist, Quaternion.LookRotation(ballDir)).GetComponent<GolemBall>();
                    golemBall.Init(growRate, rIndicator.transform.position);

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
                        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, LayerMask.GetMask("Ground")))
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

    private void OnGUI()
    {
        Vector2 scn = Camera.main.WorldToScreenPoint(transform.position);
        scn.y = Screen.height - scn.y;
       
        GUI.Label(new Rect(scn, new Vector2(150, 30)), curBehavior.ToString());
    }
}

