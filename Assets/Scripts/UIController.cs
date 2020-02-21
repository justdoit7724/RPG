using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[System.Serializable]
public struct SelectInfo
{
    public int kind;
    public RectTransform obj;
    public RectTransform content;
    public Image activeButtonImage;
}


public class UIController : MonoBehaviour
{
    [Header("UI")]
    public Sprite activeButtonSprite;
    public Sprite unactiveButtonSprite;
    public Sprite equipButtonSprite;
    public Sprite unequipButtonSprite;
    public ScrollRect selectPanel;
    public SelectInfo[] selections;

    [Header("Scene Change")]
    public RawImage sceneChangeImageInside;
    public RawImage sceneChangeImageOutside;

    const int HAIR_IDX = 0;
    const int CLOTH_IDX = 1;
    const int SWORD_IDX = 2;
    const int SHOULDERPAD_IDX = 3;

    private Image[] hairImages;
    private Image[] clothImages;
    private Image[] swordImages;
    private Image[] shoulderPadImages;
    private int curHairIdx = 0;
    private int curClothIdx = 0;
    private int curSwordIdx = 0;
    private int curShoulderPadIdx = 0;

    private int curSelectIdx = 0;

    void Start()
    {
        sceneChangeImageInside.gameObject.SetActive(false);
        sceneChangeImageOutside.gameObject.SetActive(false);

        foreach(var item in selections)
        {
            switch (item.kind)
            {
                case HAIR_IDX:
                    hairImages = item.content.GetComponentsInChildren<Image>();
                    break;
                case CLOTH_IDX:
                    clothImages = item.content.GetComponentsInChildren<Image>();
                    break;
                case SWORD_IDX:
                    swordImages = item.content.GetComponentsInChildren<Image>();
                    break;
                case SHOULDERPAD_IDX:
                    shoulderPadImages = item.content.GetComponentsInChildren<Image>();
                    break;
            }
        }

        SetNewSelect(curSelectIdx);
        //SetNewItem(HAIR_IDX, curHairIdx);
        //SetNewItem(CLOTH_IDX, curClothIdx);
        //SetNewItem(SWORD_IDX, curSwordIdx);
        //SetNewItem(SHOULDERPAD_IDX, curShoulderPadIdx);
    }

    public void SetNewSelect(int idx)
    {
        curSelectIdx = idx;

        foreach(var item in selections)
        {
            item.obj.gameObject.SetActive(false);
            item.activeButtonImage.sprite = unactiveButtonSprite;
        }
        selectPanel.content = selections[curSelectIdx].content;
        selectPanel.viewport = selections[curSelectIdx].obj;
        selections[curSelectIdx].obj.gameObject.SetActive(true);
        selections[curSelectIdx].activeButtonImage.sprite = activeButtonSprite;
    }
    public void SetNewItem(int itemKind)
    {
        int i= 0;
        switch (itemKind)
        {
            case HAIR_IDX:
                hairImages[curHairIdx].sprite = unequipButtonSprite;
                curHairIdx = i;
                hairImages[i].sprite = equipButtonSprite;
                break;
            case CLOTH_IDX:
                clothImages[curClothIdx].sprite = unequipButtonSprite;
                curClothIdx = i;
                clothImages[i].sprite = equipButtonSprite;
                break;
            case SWORD_IDX:
                swordImages[curSwordIdx].sprite = unequipButtonSprite;
                curSwordIdx = i;
                swordImages[i].sprite = equipButtonSprite;
                break;
            case SHOULDERPAD_IDX:
                shoulderPadImages[curShoulderPadIdx].sprite = unequipButtonSprite;
                curShoulderPadIdx = i;
                shoulderPadImages[i].sprite = equipButtonSprite;
                break;
        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Play()
    {
        StartCoroutine(IE_SceneChange());

    }

    private IEnumerator IE_SceneChange()
    {

        sceneChangeImageInside.gameObject.SetActive(true);
        sceneChangeImageOutside.gameObject.SetActive(true);

        float alpha = 0;
        while(alpha<1.0f)
        {
            Color mInsideCol = sceneChangeImageInside.color;
            Color mOutsideCol = sceneChangeImageOutside.color;
            mInsideCol.a = alpha;
            mOutsideCol.a = alpha;

            sceneChangeImageInside.color = mInsideCol;
            sceneChangeImageOutside.color = mOutsideCol;

            alpha += 0.01f;

            yield return null;
        }

        SceneManager.LoadScene(1);
    }

    private void OnDestroy()
    {
        PlayerPrefs.DeleteAll();
    }
}
