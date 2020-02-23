using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileTouch : MonoBehaviour
{
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(Input.touches[0].phase)
        {
            case TouchPhase.Began:
                break;
            case TouchPhase.Ended:
                break;
            case TouchPhase.Moved:
                break;
        }
    }
}
