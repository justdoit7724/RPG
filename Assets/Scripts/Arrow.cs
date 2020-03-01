using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float speed;
    private float damage;
    private int opponentLayer;
    private float lifeTime = 3;

    public void Fire(float speed, float damage, int opponentLayer)
    {
        this.speed = speed;
        this.damage = damage;
        this.opponentLayer = opponentLayer;

        enabled = true;
    }

    private void Awake()
    {
        enabled = false;
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(this);
            return;
        }

        transform.position = transform.position + (transform.forward*speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == opponentLayer)
        {
            Mob hitMob = other.gameObject.GetComponent<Mob>();
            if(hitMob)
            {
                hitMob.GetDamaged(damage);

                switch(UnityEngine.Random.Range(0,3))
                {
                    case 0:
                        hitMob.PlayHitSound("Hit1",0.15f);
                        break;
                    case 1:
                        hitMob.PlayHitSound("Hit2",0.15f);
                        break;
                    case 2:
                        hitMob.PlayHitSound("Hit3",0.15f);
                        break;
                }

                Destroy(gameObject, 0.02f);
            }
        }
    }

    private void OnDestroy()
    {
        // effect
    }
}
