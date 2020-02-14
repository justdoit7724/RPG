using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Mob : MonoBehaviour
{
    [SerializeField] protected float maxHP=1000;
    [SerializeField] protected Material mainMat;
    protected float curHP=0;

    protected Collider mainCollider;
    protected Rigidbody rigid;
    protected NavMeshAgent nav;
    protected Animator anim;

    private float curDamageEffectTime=0;

    public virtual void GetDamaged(float amount)
    {
        DamageEffect();
    }
        
    public NavMeshAgent Nav { get { return nav; } }
    public Animator Anim { get { return anim; } }
    public Rigidbody Rigid { get { return rigid; } }

    public virtual void AE_StartAttack() { }
    public virtual void AE_EndAttack() { }

    public virtual void Start()
    {
        mainCollider = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        mainMat = new Material(mainMat);

        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(var item in renderers)
        {
            item.material= mainMat;
        }

        curHP = maxHP;
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
