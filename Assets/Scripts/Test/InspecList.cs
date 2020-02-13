using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspecList : MonoBehaviour
{
    [SerializeField] private List<GameObject> lists = new List<GameObject>();

    public  GameObject this[int i]
    {
        get { return lists[i]; }
    }
}
