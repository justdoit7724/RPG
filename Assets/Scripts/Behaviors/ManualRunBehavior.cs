using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRunBehavior : BaseBehavior
{
    private float speed;

    public override void Init(BehaviorPriority p, object data, bool isAlone)
    {
        base.Init(p, data, isAlone);

        speed = (float)data;
    }

    public override void StartBehavior(Mob mob)
    {
        base.StartBehavior(mob);

        mob.Anim.SetTrigger("run");
    }

    public override bool UpdateBehavior(Mob mob)
    {
#if UNITY_EDITOR
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        Vector3 moveDir =
            Input.GetAxisRaw("Horizontal") * camRight +
            Input.GetAxisRaw("Vertical") * camForward;
        Vector3 dir = moveDir.normalized;
#elif UNITY_ANDROID

#endif

        mob.Rigid.velocity = dir * speed;

        mob.transform.LookAt(mob.transform.position + dir, Vector3.up);

        return (mob.Rigid.velocity.sqrMagnitude > 0.0001f);
    }

    public override void EndBehavior(Mob mob)
    {
        base.EndBehavior(mob);
        mob.Rigid.velocity = Vector3.zero;
        mob.Anim.ResetTrigger("run");
    }
}
