using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Mob : MonoBehaviour
{
    protected NavMeshAgent nav;

    public virtual void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }
}
