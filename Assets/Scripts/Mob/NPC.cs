using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : Mob
{
    [SerializeField] protected float detectRad = 25;
    [SerializeField] private GameObject hpBarPrefab;
    protected Transform uiCanvas;

    protected RunBehaviorData runBehaviorData;
    protected struct HPBarInfo
    {
        public Slider slider;
        public RectTransform transform;

        public HPBarInfo(GameObject instance)
        {
            slider = instance.GetComponent<Slider>();
            transform = instance.GetComponent<RectTransform>();
        }
    }
    protected HPBarInfo hpBar;
    protected Mob target = null;

    protected FSM fsm;

    private float curUpdateTargetTime = 0;
    private float updateTargetInterval = 3.0f;

    public override void Start()
    {
        base.Start();

        uiCanvas = FindObjectOfType<Canvas>().transform;

        hpBar = new HPBarInfo(Instantiate(hpBarPrefab, uiCanvas));

        fsm = GetComponent<FSM>();

        runBehaviorData = new RunBehaviorData(transform.position);

        enabled = false;
    }

    public void Init(float delayTime)
    {
        StartCoroutine(IE_Init(delayTime));
    }

    private IEnumerator IE_Init(float d)
    {
        yield return new WaitForSeconds(d);

        enabled = true;
    }

    public override void GetDamaged(float amount)
    {
        base.GetDamaged(amount);

        // die this time
        if (curHP > 0 && curHP <= amount)
        {
            Destroy(hpBar.transform.gameObject);
            BaseBehavior deathBehavior = ScriptableObject.CreateInstance<DieBehavior>();
            deathBehavior.Init(BehaviorPriority.Vital, null, true);
            fsm.CheckAndAddBehavior(deathBehavior);
        }

        curHP -= amount;
    }

    protected void UpdateHPBar()
    {
        if (curHP < maxHP)
        {
            Vector3 hpBarWPos = transform.position + Vector3.up * 2.0f;
            Vector3 scnPos = Camera.main.WorldToScreenPoint(hpBarWPos);
            scnPos.y -= Screen.height;
            hpBar.transform.anchoredPosition = scnPos;
            hpBar.slider.value = curHP / maxHP;
        }
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
