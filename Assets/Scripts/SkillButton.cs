using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public float cooltime = 15;
    
    private RawImage image;
    private RectTransform rTransform;
    private float curTime = 0;
    private bool isReady = true;
    private Vector3 oriPos;

    void Awake()
    {
        image = GetComponent<RawImage>();
        image.material.SetFloat("_Value", 0);

        rTransform = GetComponent<RectTransform>();
        oriPos = rTransform.position;
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

    public void Active(bool b)
    {
        rTransform.position = (b?oriPos:new Vector3(10000,0,0));
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
