using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBall : MonoBehaviour
{
    public GameObject golemHitEffectPrefab;
    public GameObject rangeIndicatorPrefab;
    public GameObject golemPrefab;
    public float speed=5;
    public Vector2 damage = new Vector2(80, 250);
    public Vector2 splashRad = new Vector2(7.5f, 20.0f);

    private RangeIndicator rIndicator;
    private float mDamage;
    private float mSplashRad;
    private float growRate;
    private float spawnHeight;

    public void Init(float growRate, Vector3 expPt)
    {
        this.growRate = growRate;
        mDamage = Mathf.Lerp(damage.x, damage.y, growRate);
        mSplashRad = Mathf.Lerp(splashRad.x, splashRad.y, growRate);

        spawnHeight = transform.position.y;
        rIndicator = Instantiate(rangeIndicatorPrefab, expPt, Quaternion.identity).GetComponent<RangeIndicator>();
        rIndicator.Init(mSplashRad, Color.red, 360);



    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        rIndicator.SetProgress((spawnHeight - transform.position.y) / spawnHeight);
    }

    private void OnTriggerEnter(Collider other)
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, mSplashRad, LayerMask.GetMask("Enemy"));
        foreach (var item in colls)
        {
            NPC target = item.GetComponent<NPC>();
            if (target)
            {
                float distRate = 1.0f - ((transform.position - item.transform.position).magnitude / mSplashRad);
                float curDamage = distRate * mDamage;
                target.GetDamaged(curDamage);
            }
        }

        Vector3 spawnPos = transform.position;
        spawnPos.y = 0;
        Golem golem = Instantiate(golemPrefab, spawnPos, Quaternion.identity).GetComponent<Golem>();
        golem.Init(growRate);
        Instantiate(golemHitEffectPrefab, rIndicator.transform.position, Quaternion.identity);

        Destroy(rIndicator.gameObject);
        Destroy(gameObject);
    }
}
