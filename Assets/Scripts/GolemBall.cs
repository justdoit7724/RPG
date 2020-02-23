using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemBall : MonoBehaviour
{
    public GameObject golemHitEffectPrefab;
    public GameObject rangeIndicatorPrefab;
    public GameObject golemPrefab;
    public float speed=5;
    public Vector2 damage = new Vector2(100, 240);
    public Vector2 splashRad = new Vector2(3.5f, 7.0f);
    public Vector2 hitScaleRange = new Vector2(0.4f, 0.65f);

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
        if (other.gameObject.layer != LayerMask.NameToLayer("PlayGround"))
            return;

        Collider[] colls = Physics.OverlapSphere(transform.position, mSplashRad, LayerMask.GetMask("Enemy"));
        foreach (var item in colls)
        {
            NPC target = item.GetComponent<NPC>();
            if (target)
            {
                Vector3 subVec = target.transform.position - transform.position;
                float distRate = 1.0f - (subVec.magnitude / mSplashRad);
                float curDamage = distRate * mDamage;
                target.GetDamaged(curDamage);
                target.Rigid.AddForce(subVec.normalized * distRate * 350.0f);
            }
        }

        Vector3 spawnPos = transform.position;
        spawnPos.y = 0;
        Golem golem = Instantiate(golemPrefab, spawnPos, Quaternion.identity).GetComponent<Golem>();
        golem.Init(growRate);
        Instantiate(golemHitEffectPrefab, rIndicator.transform.position, Quaternion.identity).transform.localScale = Vector3.one * Mathf.Lerp(hitScaleRange.x, hitScaleRange.y, growRate);

        Destroy(rIndicator.gameObject);
        Destroy(gameObject);
    }
}
