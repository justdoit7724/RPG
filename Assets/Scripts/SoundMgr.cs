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

        DontDestroyOnLoad(gameObject);
    }

    public void Add(string name, AudioClip clip)
    {
        audios.Add(name, clip);
    }
    public AudioClip Get(string name)
    {
        return audios[name];
    }
}
