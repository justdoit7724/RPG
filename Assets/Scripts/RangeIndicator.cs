﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    Material mat;

    public void Init(float maxRad, Color c, float viewAngle)
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        mat = new Material(meshRenderer.material);
        meshRenderer.material = mat;

        mat.SetColor("_Color", c);
        mat.SetVector("_Center", transform.position+transform.up * 0.2f);
        mat.SetFloat("_MaxRad", maxRad);
        mat.SetFloat("_ViewAngle", viewAngle);
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
    public void SetProgress(float t)
    {
        mat.SetFloat("_Value", t);
    }
}