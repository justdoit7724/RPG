using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimEventBData
{
    public string animName;
    public AudioSource soundPlayer=null;
    public string soundKey;
    public float volume;

    public AnimEventBData(string animName)
    {
        this.animName = animName;
    }
    public AnimEventBData(string animName, AudioSource soundPlayer, string soundKey, float volume)
    {
        this.animName = animName;
        this.soundPlayer = soundPlayer;
        this.soundKey = soundKey;
        this.volume = volume;
    }
}

public class AnimEventBehavior : BaseBehavior
{
    private AnimEventBData mData;

    public override void StartBehavior(Mob mob)
    {
        base.StartBehavior(mob);

        mData = (AnimEventBData)data;

            mob.Anim.SetTrigger(mData.animName);

        if(mData.soundPlayer)
        {
            SoundMgr.Instance.Play(mData.soundPlayer, mData.soundKey, mData.volume);
        }
    }

    public override bool UpdateBehavior(Mob mob)
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            return false;

        return true;
    }

    public override void EndBehavior(Mob mob)
    {
        base.EndBehavior(mob);
        //ESword bMob = mob as ESword;
        //if (bMob && bMob.Target.IsDeath())
        //{
        //    bMob.ClearTarget();
        //}
    }
}
