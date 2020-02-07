using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Mob : MonoBehaviour
{
    [SerializeField] protected float maxHP=1000;
    protected float curHP;

    protected Rigidbody rigid;
    protected NavMeshAgent nav;
    protected Animator anim;
    protected FSM fsm;

    protected RunBehaviorData walkData = new RunBehaviorData(new Vector3(0, 0, 0));

    public virtual void GetDamaged(float amount)
    {
        // die this time
        if(curHP > 0 && curHP <=amount)
        {
            BaseBehavior deathBehavior = ScriptableObject.CreateInstance<DieBehavior>();
            deathBehavior.Init(BehaviorPriority.Vital, null, true);
            fsm.AddBehavior(deathBehavior);
        }
        
        curHP -= amount;
    }
        
    public NavMeshAgent Nav { get { return nav; } }
    public Animator Anim { get { return anim; } }
    public Rigidbody Rigid { get { return rigid; } }

    public virtual void AE_StartAttack() { }
    public virtual void AE_EndAttack() { }

    public virtual void Start()
    {
        rigid = GetComponent<Rigidbody>();
        fsm = GetComponent<FSM>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        curHP = maxHP;
    }

    public bool IsDeath()
    {
        return (curHP <= 0);
    }
}
