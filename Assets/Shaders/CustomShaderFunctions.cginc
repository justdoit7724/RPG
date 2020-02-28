#ifndef __CUSTOM_SHADER_FUNCTIONS__
#define __CUSTOM_SHADER_FUNCTIONS__

#define PI 3.141592f

float Circle(float3 worldPos, float3 center, float rad)
{
    float sqrRad = rad * rad;
    
    float3 subVec = worldPos - center;
    float sqrLength = dot(subVec, subVec);
    
    return saturate(ceil(sqrRad - sqrLength));
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

float2 Proj2UV(float4 pPos)
{
    pPos.xy /= pPos.w;
    pPos.xy *= 0.5f;
    pPos.xy += 0.5f;
    
    return pPos.xy;
}

float Range01(float t, float f1, float f2)
{
    float m = (f1 + f2) * 0.5f;
    float hw = abs(f1 - f2) * 0.5f;
    
    return saturate(ceil((hw - abs(t - m)) / hw));
}
#endif