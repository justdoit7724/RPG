using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    private float damage = 0;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowFirePt;
    [SerializeField] private GameObject fireEffect;

    private Arrow curArrow = null;

    public void SetDamage(float d)
    {
        damage = d;
    }

    void Start()
    {
    }

    public void FireArrow()
    {
        curArrow = Instantiate(arrowPrefab,
            arrowFirePt.position,
            arrowFirePt.rotation).GetComponent<Arrow>();

        curArrow.Fire(16.5f, damage, LayerMask.NameToLayer("Alley"));

        Instantiate(fireEffect, transform.position,transform.rotation);
    }

}
