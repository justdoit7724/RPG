using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(FSM), typeof(Animator))]
public abstract class Mob : MonoBehaviour
{
    protected NavMeshAgent nav;
    protected Animator anim;

    public NavMeshAgent Nav { get { return nav; } }
    public Animator Anim { get { return anim; } }

    public virtual void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
}
