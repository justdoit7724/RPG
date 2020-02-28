using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LaserBData
{
    public GameObject laserPrefab;
    public Transform laserPt;
    public Mob target;
    public AudioSource soundPlayer;
    public string soundKey;
    public float volume;
    public float length;
    public float damage;

    public LaserBData(Mob target, AudioSource soundPlayer, string soundKey, float volume)
    {
        this.target = target;
        this.soundKey = soundKey;
        this.soundPlayer = soundPlayer;
        this.volume = volume;
    }
}

public class LaserBehavior : BaseBehavior
{
    private LaserBData mData;
    private Lazer laser;

    public override void StartBehavior(Mob mob)
    {
        base.StartBehavior(mob);

        mData = (LaserBData)data;

        SoundMgr.Instance.Play(mData.soundPlayer, mData.soundKey, mData.volume);

        (mob as EBoss).CreateLaser();
    }

    public override bool UpdateBehavior(Mob mob)
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            return false;

        if (mData.target && !mData.target.IsDeath())
        {
            Vector3 targetDir = (mData.target.transform.position - mob.transform.position).normalized;
            float dotV = Vector3.Dot(targetDir, mob.transform.forward);
            float cosRad = Mathf.Acos(dotV);
            if (cosRad < (Mathf.PI * 0.25f))
            {
                mob.transform.forward = Vector3.Lerp(mob.transform.forward, targetDir.normalized, 0.075f);
            }
            else
            {
                float signDotV = Vector3.Dot(targetDir, mob.transform.right);
                const float radSpeed = 45.0f;
                mob.transform.Rotate(Vector3.up, radSpeed * Mathf.Sign(signDotV) * Time.deltaTime);
            }
        }

        return true;
    }

    public override void EndBehavior(Mob mob)
    {
        base.EndBehavior(mob);

        (mob as EBoss).DeleteLaser();
    }
}
