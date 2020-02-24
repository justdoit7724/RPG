using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Mob : MonoBehaviour
{
    public float maxHP=1000;
    public Material mainMat;
    protected float curHP=0;

    protected Collider mainCollider;
    protected Rigidbody rigid;
    protected NavMeshAgent nav;
    protected Animator anim;
    protected AudioSource mainSoundPlayer;
    protected AudioSource[] hitSoundPlayers = new AudioSource[2];

    private float curDamageEffectTime=0;

    public void RemoveSounds()
    {
        mainSoundPlayer.Stop();
        mainSoundPlayer.enabled = false;
        foreach(var item in hitSoundPlayers)
        {
            item.Stop();
            item.enabled = false;
        }
    }

    public virtual void GetDamaged(float amount)
    {
        DamageEffect();
    }
        
    public NavMeshAgent Nav { get { return nav; } }
    public Animator Anim { get { return anim; } }
    public Rigidbody Rigid { get { return rigid; } }

    public virtual void AE_StartAttack() { }
    public virtual void AE_EndAttack() { }

    public virtual void GetMessage(Mob sender, MobMessage msg) { }
    public virtual void Start()
    {
        MobMgr.Instance.RegisterMob(this);

        mainCollider = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        mainSoundPlayer = gameObject.AddComponent<AudioSource>();
        for (int i = 0; i < 2; ++i)
        {
            hitSoundPlayers[i] = gameObject.AddComponent<AudioSource>();
        }

        mainMat = new Material(mainMat);

        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(var item in renderers)
        {
            item.material= mainMat;
        }

        curHP = maxHP;
    }

    public void PlayMainSound(string key, float volume=1.0f)
    {
        mainSoundPlayer.clip = SoundMgr.Instance.Get(key);
        mainSoundPlayer.volume = volume;
        mainSoundPlayer.Play();
    }
    public bool PlayHitSound(string key, float volume = 1.0f)
    {
        foreach (var player in hitSoundPlayers)
        {
            if (!player.isPlaying)
            {
                player.clip = SoundMgr.Instance.Get(key);
                player.volume = volume;
                player.Play();
                return true;
            }
        }

        return false;
    }

    private void DamageEffect()
    {
        if (curDamageEffectTime > 0)
            curDamageEffectTime = 0;
        else
            StartCoroutine(IE_DamageEffect());
    }

    private IEnumerator IE_DamageEffect()
    {
        const float damageEffectTime = 0.3f;
        curDamageEffectTime = 0;

        while (curDamageEffectTime < damageEffectTime)
        {
            curDamageEffectTime += Time.deltaTime;

            float t = curDamageEffectTime / damageEffectTime;
            float mt = Mathf.Pow(t - 1, 4);
            mainMat.SetFloat("_HitFlash", mt);

            yield return null;
        }

        curDamageEffectTime = 0;
    }

    public bool IsDeath()
    {
        return (curHP <= 0);
    }
}
