using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mob
{
    public Transform dest;

    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if(dest!=null)
        nav.SetDestination(dest.position);

    }
}
