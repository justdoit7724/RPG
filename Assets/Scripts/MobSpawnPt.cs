using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawnPt : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
