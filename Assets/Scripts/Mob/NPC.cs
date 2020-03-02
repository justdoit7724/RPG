using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : Mob
{
    [SerializeField] protected float detectRad = 25;
    [SerializeField] protected GameObject hpBarPrefab;
    protected Transform uiCanvas;
    protected HPBar hpBar=null;

    protected RunBehaviorData runBehaviorData;
    
    protected Mob target = null;

    protected FSM fsm;

    private float curUpdateTargetTime = 0;
    private float updateTargetInterval = 3.0f;


    public override void GetMessage(Mob sender, MobMessage msg)
    {
        switch (msg)
        {
            case MobMessage.Die:
                if (target == sender)
                {
                    target = null;
                }
                break;
            case MobMessage.CurseOn:
                hpBar.ShowUp(false);
                break;
            case MobMessage.CurseOff:
                hpBar.ShowUp(true);
                break;
        }
    }

    public override void Start()
    {
        base.Start();

        uiCanvas = FindObjectOfType<Canvas>().transform;

        if(hpBar==null)
        hpBar = Instantiate(hpBarPrefab, uiCanvas).GetComponent<HPBar>();

        fsm = GetComponent<FSM>();

        runBehaviorData = new RunBehaviorData(transform.position);
    }

    public void Init(float delayTime)
    {
        StartCoroutine(IE_Init(delayTime));
    }

    private IEnumerator IE_Init(float d)
    {
        yield return new WaitForSeconds(d);

        isUpdating = true;
    }

    public override void Die()
    {
        Destroy(hpBar.gameObject);
        BaseBehavior deathBehavior = ScriptableObject.CreateInstance<DieBehavior>();
        deathBehavior.Init(BehaviorPriority.Vital, null, 0);
        fsm.CheckAndAddBehavior(deathBehavior);
    }

    public override void GetDamaged(float amount)
    {
        base.GetDamaged(amount);

        // die this time
        if (curHP > 0 && curHP <= amount)
        {
            Die();
        }

        curHP -= amount;
    }

    protected void UpdateTarget(string layerName, ref Mob curTarget)
    {
        if (curTarget && !curTarget.IsDeath())
        {
            curUpdateTargetTime += Time.deltaTime;
            if (curUpdateTargetTime < updateTargetInterval)
                return;
            else
                curUpdateTargetTime = 0;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, detectRad, LayerMask.GetMask(layerName));
        float closestDist = float.MaxValue;
        curTarget = null;
        foreach(var item in colliders)
        { 
            Mob enemyMob = item.GetComponent<Mob>();
            if (enemyMob)
            {
                float sqrSubDist = (enemyMob.transform.position - transform.position).sqrMagnitude;
                if (sqrSubDist < closestDist)
                {
                    closestDist = sqrSubDist;
                    curTarget = enemyMob;
                }
            }
        }
    }
}
