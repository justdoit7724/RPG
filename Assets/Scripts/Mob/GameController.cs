﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Vector3 wayPoint=new Vector3(0,0,0);
    public Vector3 triggerBoxRad = new Vector3(2, 1, 1);


    [Header("Scene Change")]
    public RawImage sceneChangeInsideImage;
    public RawImage sceneChangeOutsideImage;

    private Player player;
    private bool isTriggered = false;

    // Start is called before the first frame update
    void Awake()
    {
        player = FindObjectOfType<Player>();

        MobileTouch.Instance.FindIndicator();

        StartCoroutine(IE_FadeIn());
    }

    private IEnumerator IE_FadeIn()
    {
        sceneChangeInsideImage.gameObject.SetActive(true);
        sceneChangeOutsideImage.gameObject.SetActive(true);

        float alpha = 1;
        while (alpha > 0.0f)
        {
            Color mInsideCol = sceneChangeInsideImage.color;
            mInsideCol.a = alpha;


            sceneChangeInsideImage.color = mInsideCol;

            alpha -= 0.02f;

            yield return null;
        }

        bool isInitPlayer = false;
        const float maxScale = 16;
        const float scaleTime = 1.2f;
        float curTime = 0;
        alpha = 1;
        while (alpha > 0.0f)
        {
            Color mInsideCol = sceneChangeOutsideImage.color;
            mInsideCol.a = alpha;

            sceneChangeOutsideImage.color = mInsideCol;

            float t = curTime / scaleTime;
            float mT = Mathf.Pow(t, 4);
            float scaleResult = Mathf.Lerp(1, maxScale, mT);
            sceneChangeOutsideImage.rectTransform.localScale = new Vector3(scaleResult, scaleResult, 1);

            alpha -= 0.002f;
            curTime += Time.deltaTime;

            if (t > 0.7f && !isInitPlayer)
            {
                player.Init();
                isInitPlayer = true;
            }
            if(t>1)
            {
                Destroy(sceneChangeInsideImage.gameObject);
                Destroy(sceneChangeOutsideImage.gameObject);
                break;
            }

            yield return null;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(!isTriggered)
        {
            Collider[] colls = Physics.OverlapBox(wayPoint, triggerBoxRad, Quaternion.identity, LayerMask.GetMask("Alley"));
            foreach(var item in colls)
            {
                Mob iMob = item.GetComponent<Mob>();
                if (iMob)
                {
                    WaveStart();
                    isTriggered = true;
                    break;
                }
            }
        }
    }

    public void WaveStart()
    {
        NPC[] enemies = FindObjectsOfType<NPC>();
        foreach(var elem in enemies)
        {
            EBoss elemBoss = elem as EBoss;
            if (elemBoss)
            {
                elemBoss.Init(4.0f);
            }
            else
                elem.Init(1.0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(wayPoint, triggerBoxRad * 2.0f);
    }
}
