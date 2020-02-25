using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public enum ItemKind
{
    Hair,
    Cloth,
    Sword,
    ShoulderPad
}
[System.Serializable]
public struct SelectInfo
{
    public ItemKind kind;
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

    [Header("Model")]
    public GameObject[] modelHairs;
    public GameObject[] modelCloths;
    public GameObject[] modelSwords;
    public GameObject[] modelShoulderPad;
    public Transform modelObj;

    [Header("Scene Change")]
    public RawImage sceneChangeImageInside;
    public RawImage sceneChangeImageOutside;

    private AudioSource sfxPlayer;
    private Image[] hairImages;
    private Image[] clothImages;
    private Image[] swordImages;
    private Image[] shoulderPadImages;
    private int curHairIdx = 0;
    private int curClothIdx = 0;
    private int curSwordIdx = 0;
    private int curShoulderPadIdx = 0;

    private float modelPrevYaw = 180;
    private int curSelectIdx = 0;
    private Vector3 mouseFirstPt = new Vector3(0, 0, 0);

    void Start()
    {
        sfxPlayer=GetComponent<AudioSource>();

        sceneChangeImageInside.gameObject.SetActive(false);
        sceneChangeImageOutside.gameObject.SetActive(false);

        foreach(var item in selections)
        {
            switch (item.kind)
            {
                case ItemKind.Hair:
                    {
                        hairImages = item.content.GetComponentsInChildren<Image>();
                    }
                    break;
                case ItemKind.Cloth:
                    {
                        clothImages = item.content.GetComponentsInChildren<Image>();
                    }
                    break;
                case ItemKind.Sword:
                    {
                        swordImages = item.content.GetComponentsInChildren<Image>();
                    }
                    break;
                case ItemKind.ShoulderPad:
                    {
                        shoulderPadImages = item.content.GetComponentsInChildren<Image>();
                    }
                    break;
            }
        }

        foreach (var item in modelHairs)
            item.SetActive(false);
        foreach (var item in modelCloths)
            item.SetActive(false);
        foreach (var item in modelSwords)
            item.SetActive(false);
        foreach (var item in modelShoulderPad)
            item.SetActive(false);

        SetNewSelect(curSelectIdx);
        SetHairItem(curHairIdx);
        SetClothItem(curClothIdx);
        SetSwordItem(curSwordIdx);
        SetShoulderPadItem(curShoulderPadIdx);
    }

    private void Update()
    {

#if UNITY_EDITOR
        if(Input.GetMouseButtonDown(0))
        {
            mouseFirstPt = Input.mousePosition;
        }
        else if(Input.GetMouseButton(0))
        {
            Vector3 dragSubVec =  mouseFirstPt- Input.mousePosition;

            float dragAmount = dragSubVec.x * 0.1f;
            modelObj.eulerAngles = new Vector3(0, dragAmount + modelPrevYaw, 0);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            Vector3 dragSubVec = mouseFirstPt-Input.mousePosition;

            float dragAmount = dragSubVec.x * 0.1f;
            modelPrevYaw = dragAmount + modelPrevYaw;
        }
#elif UNITY_ANDROID
        if (MobileTouch.Instance.IsOn)
        {
            Vector3 dragSubVec = MobileTouch.Instance.GetCurPt - MobileTouch.Instance.GetFirstPt;

            float dragAmount = dragSubVec.x * Time.deltaTime*15;
            modelObj.eulerAngles = new Vector3(0, dragAmount + modelPrevYaw, 0);
        }
        else
        {
            Vector3 dragSubVec = MobileTouch.Instance.GetCurPt - MobileTouch.Instance.GetFirstPt;

            float dragAmount = dragSubVec.x * Time.deltaTime*15;
            modelPrevYaw = dragAmount + modelPrevYaw;
        }
#endif

    }

    public void BT_ButtonSound()
    {
        SoundMgr.Instance.Play(sfxPlayer, "Button", 0.5f);
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
    public void SetHairItem(int i)
    {
        modelHairs[curHairIdx].SetActive(false);

        hairImages[curHairIdx].sprite = unequipButtonSprite;
        curHairIdx = i;
        hairImages[i].sprite = equipButtonSprite;
        modelHairs[curHairIdx].SetActive(true);
    }
    public void SetClothItem(int i)
    {
        modelCloths[curClothIdx].SetActive(false);

        clothImages[curClothIdx].sprite = unequipButtonSprite;
        curClothIdx = i;
        clothImages[i].sprite = equipButtonSprite;
        modelCloths[curClothIdx].SetActive(true);
    }
    public void SetSwordItem(int i)
    {
        modelSwords[curSwordIdx].SetActive(false);

        swordImages[curSwordIdx].sprite = unequipButtonSprite;
        curSwordIdx = i;
        swordImages[i].sprite = equipButtonSprite;
        modelSwords[curSwordIdx].SetActive(true);
    }
    public void SetShoulderPadItem(int i)
    {
        modelShoulderPad[curShoulderPadIdx].SetActive(false);

        shoulderPadImages[curShoulderPadIdx].sprite = unequipButtonSprite;
        curShoulderPadIdx = i;
        shoulderPadImages[i].sprite = equipButtonSprite;
        modelShoulderPad[curShoulderPadIdx].SetActive(true);
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
        AudioSource bgmPlayer = Camera.main.GetComponent<AudioSource>();

        sceneChangeImageInside.gameObject.SetActive(true);
        sceneChangeImageOutside.gameObject.SetActive(true);

        float alpha = 0;
        while(alpha<1.0f)
        {
            Color mInsideCol = sceneChangeImageInside.color;
            Color mOutsideCol = sceneChangeImageOutside.color;
            mInsideCol.a = alpha;
            mOutsideCol.a = alpha;

            bgmPlayer.volume = 1.0f - alpha;

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

        PlayerPrefs.SetInt("Hair", curHairIdx);
        PlayerPrefs.SetInt("Cloth", curClothIdx);
        PlayerPrefs.SetInt("Sword", curSwordIdx);
        PlayerPrefs.SetInt("ShoulderPad", curShoulderPadIdx);
    }
}
