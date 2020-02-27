using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovePt : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.15f);
    }
}
