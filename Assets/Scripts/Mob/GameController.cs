using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : MonoBehaviour
{
    public Vector3 wayPoint=new Vector3(0,0,0);
    public Vector3 triggerBoxRad = new Vector3(2, 1, 1);

    private bool isTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isTriggered)
        {
            if(Physics.OverlapBox(wayPoint, triggerBoxRad, Quaternion.identity, LayerMask.GetMask("Alley")).Length>0)
            {
                GameStart();
                isTriggered = true;
            }
        }
    }

    public void GameStart()
    {
        FindObjectOfType<Player>().Init();

        NPC[] enemies = FindObjectsOfType<NPC>();
        foreach(var elem in enemies)
        {
            EBoss elemBoss = elem as EBoss;
            if (elemBoss)
            {
                elemBoss.Init(4.0f);
            }
            else
                elem.Init(1.0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(wayPoint, triggerBoxRad * 2.0f);
    }
}
