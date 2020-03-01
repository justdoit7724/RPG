using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour
{
    private static SoundMgr instance = null;
    public static SoundMgr Instance {
        get {
            if(instance==null)
            {
                instance = new GameObject("SoundMgr").AddComponent<SoundMgr>();
            }
            return instance;
        }
    }

    private Dictionary<string, AudioClip> audios = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        audios.Add("Button", Resources.Load("Audios/Button") as AudioClip);
        audios.Add("PlayerAtt1", Resources.Load("Audios/PlayerAtt1") as AudioClip);
        audios.Add("PlayerAtt2", Resources.Load("Audios/PlayerAtt2") as AudioClip);
        audios.Add("EnemyAtt", Resources.Load("Audios/EnemyAtt") as AudioClip);
        audios.Add("EnemyAttDouble", Resources.Load("Audios/EnemyAttDouble") as AudioClip);
        audios.Add("BowAtt", Resources.Load("Audios/BowAtt") as AudioClip);
        audios.Add("Laser", Resources.Load("Audios/Laser") as AudioClip);
        audios.Add("BossBallAura", Resources.Load("Audios/BossBallAura") as AudioClip);
        audios.Add("BossSpawnAura1", Resources.Load("Audios/BossSpawnAura1") as AudioClip);
        audios.Add("BossSpawnAura2", Resources.Load("Audios/BossSpawnAura2") as AudioClip);
        audios.Add("BossSpawnMobAura1", Resources.Load("Audios/BossSpawnMobAura1") as AudioClip);
        audios.Add("BossSpawnMobAura2", Resources.Load("Audios/BossSpawnMobAura2") as AudioClip);
        audios.Add("LaserCharge", Resources.Load("Audios/LaserCharge") as AudioClip);
        audios.Add("Hit1", Resources.Load("Audios/Hit1") as AudioClip);
        audios.Add("Hit2", Resources.Load("Audios/Hit2") as AudioClip);
        audios.Add("Hit3", Resources.Load("Audios/Hit3") as AudioClip);
        audios.Add("BossBallHit", Resources.Load("Audios/BossBallHit") as AudioClip);
        audios.Add("GolemHit", Resources.Load("Audios/GolemHit") as AudioClip);
        audios.Add("GolemHit2", Resources.Load("Audios/GolemHit2") as AudioClip);
        audios.Add("GolemHit3", Resources.Load("Audios/GolemHit3") as AudioClip);
        audios.Add("Sliding", Resources.Load("Audios/Sliding") as AudioClip);
        audios.Add("PlayerBallSpawnAndHit", Resources.Load("Audios/PlayerBallSpawnAndHit") as AudioClip);
        audios.Add("PlayerBallSwing", Resources.Load("Audios/PlayerBallSwing") as AudioClip);
        audios.Add("FootStep", Resources.Load("Audios/FootStep") as AudioClip);
        audios.Add("GolemStep", Resources.Load("Audios/GolemStep") as AudioClip);

        DontDestroyOnLoad(gameObject);
    }

    public void PlayInstance(Vector3 pos, string key, float volume, float lifeTime)
    {
        AudioSource obj = new GameObject("SoundInstance").AddComponent< AudioSource>();
        obj.transform.position = pos;
        obj.clip = audios[key];
        obj.volume = volume;
        obj.Play();

        Destroy(obj.gameObject, lifeTime);
    }
    public void PlayInstanceFadeOut(Vector3 pos, string key, float firstVolume, float lifeTime)
    {
        AudioSource obj = new GameObject("SoundInstance").AddComponent<AudioSource>();
        obj.transform.position = pos;
        obj.clip = audios[key];
        obj.volume = firstVolume;
        obj.Play();

        StartCoroutine(IE_PlayFadeOut(obj, firstVolume, lifeTime,true));
    }
    public void Play(AudioSource player, string key, float volume)
    {
        if (!audios.ContainsKey(key))
        {
            Debug.LogError(key + "'sound is not exist");
        }
        else
        {
            if(player.enabled==false)
            {
                int a = 0;
                a++;
            }
            player.clip = audios[key];
            player.volume = volume;
            player.Play();
        }
    }

    private static float CurveT(float x)
    {
        return (-Mathf.Pow(x, 4) + 1);
    }

    public void PlayFadeOut(AudioSource player, string key, float firstVolume, float time)
    {
        player.clip = audios[key];
        player.volume = firstVolume;
        player.Play();
        StartCoroutine(IE_PlayFadeOut(player, firstVolume, time,false));
    }
    private IEnumerator IE_PlayFadeOut(AudioSource player, float firstVolume, float time, bool isDestroyOwner)
    {
        float mt = 0;
        float curTime = 0;
        while(mt<1.0f)
        {
            curTime += Time.deltaTime;
            mt = CurveT(curTime / time);

            player.volume = mt * firstVolume;

            yield return null;
        }

        if (isDestroyOwner)
            Destroy(player.gameObject);
    }
}
