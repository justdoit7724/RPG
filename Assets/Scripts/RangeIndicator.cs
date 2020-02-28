using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    private Material mat;
    private bool isProgressing = false;

    public void Init(float maxRad, Color c, float viewAngle)
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        mat = new Material(meshRenderer.material);
        meshRenderer.material = mat;

        mat.SetColor("_Color", c);
        mat.SetVector("_Center", transform.position+transform.up * 0.2f);
        mat.SetFloat("_MaxRad", maxRad);
        mat.SetFloat("_ViewAngle", viewAngle);

        gameObject.SetActive(false);
    }

    public void SetMaxRad(float r)
    {
        mat.SetFloat("_MaxRad", r);
    }
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos + transform.up * 0.2f;
        mat.SetVector("_Center", pos + transform.up * 0.2f);
    }
    public void SetDir(Vector3 dir)
    {
        mat.SetVector("_ViewDir", dir);
    }
    public void SetProgress(float t)
    {
        mat.SetFloat("_Value", t);
    }

    public void StartProgress(float time, bool destroy)
    {
        gameObject.SetActive(true);

        if(!isProgressing)
            StartCoroutine(IE_Progress(time, destroy));
    }

    public IEnumerator IE_Progress(float time, bool destroy)
    {
        isProgressing = true;

        float curTime = 0;

        while(curTime< time)
        {
            curTime += Time.deltaTime;

            mat.SetFloat("_Value", curTime / time);

            yield return null;
        }

        isProgressing = false;

        if (destroy)
            Destroy(gameObject);
        else
        {
            gameObject.SetActive(false);
        }
    }
}
