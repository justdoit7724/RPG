using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float rad;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, rad, LayerMask.GetMask("Ground", "Wall"));
        Debug.Log(colls.Length);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rad);
    }
}
