using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBow : Mob
{
    [SerializeField] private float detectRad = 30;
    [SerializeField] private float attRad = 15;
    [SerializeField] private GameObject visualArrow;
    private float sqrAttRad;
    private Mob target = null;
    private WeaponTrail trail;
    private Bow weapon;
    private BaseBehavior attBehavior;
    private ArrowAttBehaviorData attData = new ArrowAttBehaviorData(null, 3.5f);

    public Mob Target { get { return target; } }

    public override void Start()
    {
        base.Start();

        sqrAttRad = attRad * attRad;

        trail = GetComponentInChildren<WeaponTrail>();
        weapon = GetComponentInChildren<Bow>();

        attBehavior = ScriptableObject.CreateInstance<ArrowAttBehavior>();
       
        attBehavior.Init(BehaviorPriority.Skill, attData, true);
    }

    public override void AE_StartAttack()
    {
            trail.StartTrail();
    }
    public void AE_Fire()
    {
        weapon.FireArrow();
        visualArrow.SetActive(false);
    }
    public override void AE_EndAttack()
    {
        trail.EndTrail();
        visualArrow.SetActive(true);
    }

    void Update()
    {
        if (target == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectRad, LayerMask.GetMask("Player"));
            if (colliders.Length > 0)
            {
                target = colliders[0].GetComponent<Mob>();
            }
            else
                fsm.AddBehavior(idleBehavior);
        }

        if (target)
        {
            Vector3 subVec = transform.position - target.transform.position;
            if (subVec.sqrMagnitude <= sqrAttRad)
            {
                attData.target = target.transform;
                fsm.AddBehavior(attBehavior);
            }
            else
            {
                walkData.dest = target.transform.position;
                fsm.AddBehavior(walkBehavior);
            }
            
        }
    }

    public void ClearTarget()
    {
        target = null;
    }
}
