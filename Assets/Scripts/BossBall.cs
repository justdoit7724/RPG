using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBall : MonoBehaviour
{
    public GameObject HitEffectPrefab;
    public float damage = 50.0f;
    public float speed = 5.0f;
    public float lifeTime = 4.0f;

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
            Instantiate(HitEffectPrefab, transform.position, Quaternion.identity);
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Alley") || other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            NPC hitMob = other.GetComponent<NPC>();
            if(hitMob)
            {
                hitMob.GetDamaged(damage);
            }

            Destroy(gameObject);
            Instantiate(HitEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}
