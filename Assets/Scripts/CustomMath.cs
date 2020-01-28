using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMath
{
    public static bool Intersection_Plane_Ray(Plane plane, Ray ray, out Vector3 point)
    {
        if(Vector3.Dot(ray.direction, plane.normal)==0)
        {
            point = new Vector3(0,0,0);
            return false;
        }

        float t = Vector3.Dot(plane.normal * plane.distance - ray.origin, plane.normal) / Vector3.Dot(ray.direction, plane.normal);
        point = ray.origin + ray.direction * t;
        return true;
    }
    public static Vector3 BezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return (1 - t) * (1 - t) * p0 + (2 * t - 2 * t * t) * p1 + t * t * p2;
    }
}
