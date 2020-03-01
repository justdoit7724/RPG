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

    private Player player;
    private RangeIndicator rIndicator;
    private float mDamage;
    private float mSplashRad;
    private float growRate;
    private float spawnHeight;

    public void Init(Player player, float growRate, Vector3 expPt)
    {
        this.player = player;
        this.growRate = growRate;
        mDamage = Mathf.Lerp(damage.x, damage.y, growRate);
        mSplashRad = Mathf.Lerp(splashRad.x, splashRad.y, growRate);

        spawnHeight = transform.position.y;
        rIndicator = Instantiate(rangeIndicatorPrefab, expPt, Quaternion.identity).GetComponent<RangeIndicator>();
        rIndicator.Init(mSplashRad, Color.red, 360);
        rIndicator.gameObject.SetActive(true);

        StartCoroutine(IE_PlayFallingSound(Mathf.Lerp(0, 2.0f, growRate)));
    }

    private IEnumerator IE_PlayFallingSound(float delay)
    {
        yield return new WaitForSeconds(delay);

        GetComponent<AudioSource>().Play();
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
        float physicPower = Mathf.Lerp(350f, 500.0f, growRate);
        foreach (var item in colls)
        {
            NPC target = item.GetComponent<NPC>();
            if (target)
            {
                Vector3 subVec = target.transform.position - transform.position;
                float distRate = 1.0f - (subVec.magnitude / mSplashRad);
                float curDamage = distRate * mDamage;
                target.GetDamaged(curDamage);
                target.Rigid.AddForce(subVec.normalized * distRate * physicPower);
            }
        }

        Vector3 spawnPos = transform.position;
        spawnPos.y = 0;
        player.SpawnGolem(spawnPos);
        GameObject ballObj = Instantiate(golemHitEffectPrefab, rIndicator.transform.position, Quaternion.identity);
        ballObj.transform.localScale = Vector3.one * Mathf.Lerp(hitScaleRange.x, hitScaleRange.y, growRate);
        SoundMgr.Instance.Play(ballObj.AddComponent<AudioSource>(), "GolemHit", 1.0f);
        SoundMgr.Instance.Play(ballObj.AddComponent<AudioSource>(), "GolemHit2", 1.0f);
        SoundMgr.Instance.Play(ballObj.AddComponent<AudioSource>(), "GolemHit3", 1.0f);

        Destroy(rIndicator.gameObject);
        Destroy(gameObject);
    }
}
