#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

// （根据法线和入射光的夹角）获取表面进光
float3 IncomingLight(Surface surface, Light light)
{
    return saturate(dot(surface.normal, light.direction)) * light.color;
}

// 获取光与物体表面作用后的颜色
float3 GetLighting(Surface surface, BRDF brdf, Light light)
{
    return IncomingLight(surface, light) * DirectBRDF(surface, brdf, light);
}

// 获取光与物体表面作用后的颜色
float3 GetLighting(Surface surface, BRDF brdf)
{
    float3 color = 0.0;
    for (int i = 0; i < GetDirectionalLightCount(); ++i)
    {
        color += GetLighting(surface, brdf, GetDirectionalLight(i));
    }
    return color;
}

#endif