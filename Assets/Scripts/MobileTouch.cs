using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileTouch : MonoBehaviour
{
    private static MobileTouch instance;
    public static MobileTouch Instance {
        get {
            if(instance==null)
            {
                instance = new GameObject("TouchController").AddComponent<MobileTouch>();
            }
            return instance;
        }
    }

    private Material indicatorMat = null;
    private Vector2 scnOffset = new Vector2(540, 1140);

public void FindIndicator()
    {
        indicatorMat = GameObject.FindWithTag("TouchIndicator").GetComponent<RawImage>().material;
        indicatorMat.SetVector("_StartPt", new Vector4(-10000, 0, 0, 0));
        indicatorMat.SetVector("_EndPt", new Vector4(-10000, 0, 0, 0));
    }

    public TouchPhase GetTouchPhase
    {
        get {
            return phase;
        }
    }
    public Vector3 GetFirstPt {
        get {
            return firstTouchPt;
        }
    }
    public Vector3 GetCurPt {
        get {
            return Input.touches[0].position;
        }
    }
    public float SqrStationaryDragDist() {
        
        return (stationaryTouchPt - GetCurPt).sqrMagnitude;
    }
    public float StationaryDragTime()
    {
        return Time.time - stationaryTouchTime;
    }
    public float SqrFirstDragDist()
    {

        return (firstTouchPt - GetCurPt).sqrMagnitude;
    }
    public float FirstDragTime()
    {
        return Time.time - firstTouchTime;
    }
    //drag direction in pixel space
    public Vector3 DragDir()
    {
        Vector3 subVec = GetCurPt - firstTouchPt;
        return subVec.normalized;
    }
    public bool IsOnLobby {
        get {
            return (
                Input.touchCount > 0 &&
                Input.touches[0].position.y>380.0f);
        }
    }
    public bool IsOnPlay {
        get {
            return (
                Input.touchCount > 0 &&
                (Input.touches[0].position.x > 260.0f ||
                Input.touches[0].position.y > 160.0f));
        }
    }
    public bool IsOn {
        get {
            return (Input.touchCount > 0);
        }
    }

    private TouchPhase phase= TouchPhase.Canceled;
    private Vector3 firstTouchPt = new Vector3(0, 0, 0);
    private float firstTouchTime = 0;
    private Vector3 stationaryTouchPt=new Vector3(0,0,0);
    private float stationaryTouchTime = 0;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        DontDestroyOnLoad(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0)
            return;

        phase = Input.touches[0].phase;
        switch (phase)
        {
            case TouchPhase.Began:
                firstTouchTime = Time.time;
                firstTouchPt= Input.touches[0].position;
                stationaryTouchTime = Time.time;
                stationaryTouchPt = Input.touches[0].position;
                if (indicatorMat)
                {
                    indicatorMat.SetVector("_StartPt", new Vector4(firstTouchPt.x - scnOffset.x, firstTouchPt.y - scnOffset.y, 0, 0));
                    indicatorMat.SetVector("_EndPt", new Vector4(firstTouchPt.x - scnOffset.x, firstTouchPt.y - scnOffset.y, 0, 0));
                }
                break;
            case TouchPhase.Ended:
                if (indicatorMat)
                {
                    indicatorMat.SetVector("_StartPt", new Vector4(-100 - scnOffset.x, -scnOffset.y, 0, 0));
                    indicatorMat.SetVector("_EndPt", new Vector4(-100 - scnOffset.x, -scnOffset.y, 0, 0));
                }
                break;
            case TouchPhase.Moved:
                if (indicatorMat)
                {
                    indicatorMat.SetVector("_EndPt", new Vector4(Input.touches[0].position.x - scnOffset.x, Input.touches[0].position.y - scnOffset.y, 0, 0));
                }
                break;
            case TouchPhase.Stationary:
                stationaryTouchTime = Time.time;
                stationaryTouchPt = Input.touches[0].position;
                if (indicatorMat)
                {
                    indicatorMat.SetVector("_EndPt", new Vector4(stationaryTouchPt.x - scnOffset.x, stationaryTouchPt.y - scnOffset.y, 0, 0));
                }
                break;
        }
    }
}
