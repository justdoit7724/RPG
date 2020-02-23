using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public float cooltime = 15;

    private RawImage image;
    private float curTime = 0;
    private bool isReady = true;

    void Start()
    {
        image = GetComponent<RawImage>();
        image.material.SetFloat("_Value", 0);
    }
    private void Update()
    {
        if (!isReady)
        {
            curTime += Time.deltaTime;

            image.material.SetFloat("_Value", (cooltime - curTime) / cooltime);

            if (curTime >= cooltime)
                isReady = true;
        }
    }

    public void StartCooldown()
    {
        isReady = false;
        curTime = 0;
    }
    public bool IsReady
    {
        get { return isReady; }
    }
}
