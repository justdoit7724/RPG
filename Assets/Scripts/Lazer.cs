using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lazer : MonoBehaviour
{
    public GameObject hitEffectPrefab;
    private float damage;
    private float interval;
    private float curTime = 0;
    private bool isReadyToHit = true;

    public void Init(float length, float damage, float interval)
    {
        LineRenderer[] lines = GetComponentsInChildren<LineRenderer>();
        foreach(var item in lines)
        {
            item.SetPosition(1, new Vector3(0, 0, length+1));
        }
        CapsuleCollider coll = GetComponent<CapsuleCollider>();
        coll.height = length;
        coll.center = new Vector3(0, 0, length * 0.5f);

        this.damage = damage;
        this.interval = interval;
        
    }

    private void Update()
    {
        curTime += Time.deltaTime;

        if (curTime >= interval)
        {
            curTime = 0;
            isReadyToHit = true;
        }
        else
            isReadyToHit = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isReadyToHit)
            return;

        Mob mob = other.GetComponent<Mob>();
        if(mob)
        {
            mob.GetDamaged(damage);

            Instantiate(hitEffectPrefab, other.transform.position, Quaternion.identity);
        }
    }
}
