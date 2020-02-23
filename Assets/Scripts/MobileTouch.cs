using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public TouchPhase GetTouchPhase
    {
        get {
            return phase;
        }
    }
    public Vector3 GetFirstPt {
        get {
            return stationaryTouchPt;
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

        switch (Input.touches[0].phase)
        {
            case TouchPhase.Began:
                firstTouchTime = Time.time;
                firstTouchPt= Input.touches[0].position;
                stationaryTouchTime = Time.time;
                stationaryTouchPt = Input.touches[0].position;
                break;
            case TouchPhase.Ended:
                break;
            case TouchPhase.Moved:
                break;
            case TouchPhase.Stationary:
                stationaryTouchTime = Time.time;
                stationaryTouchPt = Input.touches[0].position;
                break;
        }
        phase = Input.touches[0].phase;
    }
}
