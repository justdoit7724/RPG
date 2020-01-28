using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mob
{
    WeaponTrail trail;
    float temp = 0;

    public override void Start()
    {
        base.Start();

        trail = GetComponentInChildren<WeaponTrail>();
    }

    public void StartTrail()
    {
        trail.StartTrail();
    }
    public void EndTrail()
    {
        trail.EndTrail();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
