using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MeleeWeapon : MonoBehaviour
{
    private float damage=0;

    private Collider weaponCollider;

    private int layer;
    private int opponentLayer;

    public void SetDamage(float d)
    {
        damage = d;
    }

    private void Start()
    {
        weaponCollider = GetComponent<Collider>();

        layer = gameObject.layer;
        switch (LayerMask.LayerToName(layer))
        {
            case "Alley":
                opponentLayer = LayerMask.NameToLayer("Enemy");
                break;
            case "Enemy":
                opponentLayer = LayerMask.NameToLayer("Alley");
                break;
        }

        enabled = false;
        weaponCollider.enabled = false;
    }

    private IEnumerator IE_Disable()
    {
        yield return new WaitForSeconds(1.0f);

        enabled = false;
        weaponCollider.enabled = false;
    }


    private void OnEnable()
    {
        if(weaponCollider)
            weaponCollider.enabled = true;

        StartCoroutine(IE_Disable());
    }
    private void OnDisable()
    {
        weaponCollider.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == opponentLayer)
        {
            Mob hitMob = other.GetComponent<Mob>();
            if(hitMob)
            {
                hitMob.GetDamaged(damage);
            }
        }
    }
}
