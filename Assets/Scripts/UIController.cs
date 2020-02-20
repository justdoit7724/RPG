using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct SelectInfo
{
    public RectTransform obj;
    public RectTransform content;
    public Image activeButtonImage;
}

public class UIController : MonoBehaviour
{
    public Sprite activeButtonSprite;
    public Sprite unactiveButtonSprite;

    public ScrollRect selectPanel;
    public SelectInfo[] selections;

    private int curSelectIdx = 0;

    void Start()
    {
        SetNewSelect(curSelectIdx);
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

    public void Exit()
    {
        Application.Quit();
    }

    public void Play()
    {
        SceneManager.LoadScene(1,LoadSceneMode.Additive);

    }
}
