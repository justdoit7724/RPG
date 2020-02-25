using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class FireBall : MonoBehaviour
{
    public float speed=7;
    public float maxGrowTime = 1.5f;
    public float lifeTime = 3.0f;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public Vector2 splashRad = new Vector2(0.3f, 1.5f);
    public Vector2 damage = new Vector2(10, 150);
    public Vector2 scaleRange = new Vector2(0.6f, 1.0f);
    public Collider mainCollider;
    public AudioSource audioSource;

    private float curTime = 0;

    void Start()
    {
        var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
        muzzleVFX.transform.forward = gameObject.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        curTime += Time.deltaTime;
        if(curTime >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }
        float t = curTime / maxGrowTime;

        transform.localScale = Mathf.Lerp(scaleRange.x, scaleRange.y, t)*Vector3.one;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && other.GetComponent<Mob>() == null)
                return;

            float t = curTime / maxGrowTime;

            float curSplashRad = Mathf.Lerp(splashRad.x, splashRad.y, t);
            Collider[] colls = Physics.OverlapSphere(transform.position, curSplashRad, LayerMask.GetMask("Enemy"));
            foreach (var item in colls)
            {
                NPC npc = item.GetComponent<NPC>();
                if (npc == null)
                    continue;
                float curMaxDamage = Mathf.Lerp(damage.x, damage.y, t);
                float curDamage = ((transform.position - item.transform.position).magnitude / curSplashRad) * curMaxDamage;
                npc.GetDamaged(curDamage);
            }

            Instantiate(hitPrefab, transform.position, Quaternion.identity).transform.localScale= Mathf.Lerp(scaleRange.x, scaleRange.y, t) * Vector3.one;

            SoundMgr.Instance.PlayInstanceFadeOut(transform.position, "PlayerBallSpawnAndHit", 1.0f, 1.0f);
            mainCollider.enabled = false;

            Destroy(gameObject);
        }
    }

}
