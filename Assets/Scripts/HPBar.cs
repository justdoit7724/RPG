using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    private RectTransform rTransform;
    private Slider slider;
    private Image[] images;

    void Start()
    {
        rTransform = GetComponent<RectTransform>();
        slider = GetComponent<Slider>();
        images = GetComponentsInChildren<Image>();
    }

    public void ShowUp(bool b)
    {
        foreach(Image item in images)
        {
            item.enabled = b;
        }
    }

    public void UpdateBar(float curHP, float maxHP, Vector3 wPos)
    {
        if (curHP < maxHP)
        {
            gameObject.SetActive(true);
            Vector3 hpBarWPos = wPos + Vector3.up * 2.0f;
            Vector3 scnPos = Camera.main.WorldToScreenPoint(hpBarWPos);
            scnPos.y -= Screen.height;
            rTransform.anchoredPosition = scnPos;
            slider.value = curHP / maxHP;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
