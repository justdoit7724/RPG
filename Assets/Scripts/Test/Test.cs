using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float rad = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.depthTextureMode = Camera.main.depthTextureMode | DepthTextureMode.DepthNormals;
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Q))
        {
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, rad);
    }
}
