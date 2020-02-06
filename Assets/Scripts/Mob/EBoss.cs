using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBoss : Mob
{
    [SerializeField] private float detectRad = 30;
    private Mob target = null;
    private WeaponTrail trail;
    private BaseBehavior idleBehavior;
    private BaseBehavior dieBehavior;

    public override void Start()
    {
        base.Start();

        trail = GetComponentInChildren<WeaponTrail>();

        idleBehavior = ScriptableObject.CreateInstance<IdleBehavior>();
        idleBehavior.Init(BehaviorPriority.Basic, null, true);
    }

    public override void AE_StartAttack()
    {
        trail.StartTrail();
    }
    public override void AE_EndAttack()
    {
        trail.EndTrail();
    }

    private void Update()
    {
        if (IsDeath())
            return;

        if(target==null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectRad, LayerMask.GetMask("Player"));
            if (colliders.Length > 0)
                target = colliders[0].GetComponent<Mob>();
        }
        else
        {
            fsm.AddBehavior(idleBehavior);
        }
    }


}

