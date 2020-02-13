using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotDir
{
    X,Y,Z
}

public class RotationObj : MonoBehaviour
{
    public Space space=Space.World;
    public RotDir dirKind= RotDir.Y;
    [Range(-360.0f,360.0f)]
    public float speed;

    // Update is called once per frame
    void Update()
    {
        Vector3 dir=Vector3.zero;
        switch(dirKind)
        {
            case RotDir.X:
                dir = Vector3.right;
                break;
            case RotDir.Y:
                dir = Vector3.up;
                break;
            case RotDir.Z:
                dir = Vector3.forward;
                break;
        }

        transform.Rotate(dir, speed * Time.deltaTime, space);
    }
}
