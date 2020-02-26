#ifndef __LIGHTING__
#define __LIGHTING__

void ComputeDirectionalLight(float3 lDir, float3 normal, float3 toEye, out float3 lightDiffuse, out float3 lightSpec)
{
    lightDiffuse = max(0.0f, dot(-lDir, normal));
    
    float3 v = reflect(lDir, normal);
    lightSpec = pow(saturate(dot(v, toEye)), 8);
}
#endif