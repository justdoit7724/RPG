#ifndef __CUSTOM_SHADER_FUNCTIONS__
#define __CUSTOM_SHADER_FUNCTIONS__


float Circle(float3 worldPos, float3 center, float rad)
{
    float sqrRad = rad * rad;
    
    float3 subVec = worldPos - center;
    float sqrLength = dot(subVec, subVec);
    
    return saturate(ceil(rad - sqrLength));
}

float AngleBetweenDir(float3 dir1, float3 dir2)
{
    float mDot = dot(dir1, dir2);
    float rad = acos(mDot);
    return degrees(rad);
}

float3 Lerp(float3 v1, float3 v2, float t)
{
    return (v1 + (v2 - v1) * t);
}
#endif