using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class FireBall : MonoBehaviour
{
    public float speed;
    public float maxGrowTime = 1;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public Vector2 splashRad = new Vector2(0.3f, 1.5f);
    public Vector2 damage = new Vector2(10, 150);
    public Vector2 ball_mainBeamSize = new Vector2(0.8f, 1.4f);
    public Vector2 ball_glowBeamSize = new Vector2(2.5f, 4.5f);
    public Vector2 ball_smokeSize = new Vector2(0.6f, 1.0f);
    public Vector2 ball_particleEmitRad = new Vector2(0.25f, 0.4f);
    public Vector2 hit_mainBeamSize = new Vector2(0.3f, 1.5f);
    public Vector2 hit_glowBeamSize = new Vector2(0.15f, 0.5f);
    public Vector2 hit_smokeSize = new Vector2(0.3f, 2.0f);
    public Vector2 hit_particleSize = new Vector2(0.03f, 0.6f);
    public Vector2 hit_beam = new Vector2(2.0f, 10.0f);
    public Vector2 hit_circle = new Vector2(2.5f, 8.0f);


    public List<ParticleSystem> particles = new List<ParticleSystem>();

    private float curTime = 0;

    void Start()
    {
        var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
        muzzleVFX.transform.forward = gameObject.transform.forward;
        var ps = muzzleVFX.GetComponent<ParticleSystem>();
        if (ps != null)
            Destroy(muzzleVFX, ps.main.duration);
        else
        {
            var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
            Destroy(muzzleVFX, psChild.main.duration);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        curTime += Time.deltaTime;
        float t = curTime / maxGrowTime;
        MainModule p1Main = particles[0].main;
        MainModule p2Main = particles[1].main;
        MainModule p3Main = particles[2].main;
        ShapeModule p4Shape = particles[3].shape;
        p1Main.startSize = new MinMaxCurve(Mathf.Lerp(ball_mainBeamSize.x, ball_mainBeamSize.y, t));
        p2Main.startSize = new MinMaxCurve(Mathf.Lerp(ball_glowBeamSize.x, ball_glowBeamSize.y, t));
        float p3Size = Mathf.Lerp(ball_smokeSize.x, ball_smokeSize.y, t);
        p3Main.startSize = new MinMaxCurve(p3Size, p3Size + 0.4f);
        p4Shape.radius = Mathf.Lerp(ball_particleEmitRad.x, ball_particleEmitRad.y, t);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {

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

            InspecList pList = Instantiate(hitPrefab, transform.position, Quaternion.identity).GetComponent<InspecList>();
            MainModule p1Main = pList[0].GetComponent<ParticleSystem>().main;
            MainModule p2Main = pList[1].GetComponent<ParticleSystem>().main;
            MainModule p3Main = pList[2].GetComponent<ParticleSystem>().main;
            MainModule p4Main = pList[3].GetComponent<ParticleSystem>().main;
            MainModule p5Main = pList[4].GetComponent<ParticleSystem>().main;
            MainModule p6Main = pList[5].GetComponent<ParticleSystem>().main;
            p1Main.startSize = new MinMaxCurve(Mathf.Lerp(hit_mainBeamSize.x, hit_mainBeamSize.y, t));
            p2Main.startSize = new MinMaxCurve(Mathf.Lerp(hit_glowBeamSize.x, hit_glowBeamSize.y, t));
            p3Main.startSize = new MinMaxCurve(Mathf.Lerp(hit_smokeSize.x, hit_smokeSize.y, t));
            p4Main.startSize = new MinMaxCurve(Mathf.Lerp(hit_particleSize.x, hit_particleSize.y, t));
            p5Main.startSize = new MinMaxCurve(Mathf.Lerp(hit_beam.x, hit_beam.y, t));
            p6Main.startSize = new MinMaxCurve(Mathf.Lerp(hit_circle.x, hit_circle.y, t));

            Destroy(gameObject);
        }
    }

}
