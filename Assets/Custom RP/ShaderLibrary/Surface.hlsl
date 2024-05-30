#ifndef CUSTOM_SURFACE_INCLUDED
#define CUSTOM_SURFACE_INCLUDED

struct Surface
{
    float3 normal;
    // 从物体表面到摄像机的视角方向
    float3 viewDirection;
    // 物体表面颜色（主纹理采样颜色和顶点色）
    float3 color;
    float alpha;

    float metallic;
    float smoothness;
};

#endif