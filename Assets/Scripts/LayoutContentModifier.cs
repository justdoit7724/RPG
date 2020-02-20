using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class LayoutContentModifier : MonoBehaviour
{
    void Start()
    {
        float spacing = GetComponent<HorizontalLayoutGroup>().spacing;

        Button[] elems = GetComponentsInChildren<Button>();
        float elemWidth = elems[0].GetComponent<RectTransform>().sizeDelta.x;

        float panelWidth = (elemWidth + spacing) * elems.Length;

        RectTransform curRectTransform = GetComponent<RectTransform>();
        curRectTransform.sizeDelta = new Vector2(panelWidth, curRectTransform.sizeDelta.y);
        curRectTransform.anchoredPosition3D = Vector3.zero;
    }
}
