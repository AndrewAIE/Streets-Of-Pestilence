#ifndef DYNAMIC_FOG_2_COMMONS_URP
#define DYNAMIC_FOG_2_COMMONS_URP

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

// ***** Uncomment to enable orthographic camera support
//#define ORTHO_SUPPORT

// ***** Uncomment to enable alternate world space reconstruction function
//#define USE_ALTERNATE_RECONSTRUCT_API


// Common URP code
#define VR_ENABLED defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED) || defined(SINGLE_PASS_STEREO)

#if defined(USE_ALTERNATE_RECONSTRUCT_API) || VR_ENABLED 
   #define CLAMP_RAY_DEPTH(rayStart, scrPos, t1) ClampRayDepthAlt(rayStart, scrPos, t1)
#else
   #define CLAMP_RAY_DEPTH(rayStart, scrPos, t1) ClampRayDepth(rayStart, scrPos, t1)
#endif

TEXTURE2D(_CustomDepthTexture);
SAMPLER(sampler_CustomDepthTexture);
int DF2_FLIP_DEPTH_TEXTURE;
float rawDepth;

inline float3 ProjectOnPlane(float3 v, float3 planeNormal) {
    // assuming planeNormal as a modulus of 1
    float dt = dot(v, planeNormal);
	return v - planeNormal * dt;
}

inline float3 GetRayStart(float3 wpos) {
    float3 cameraPosition = GetCameraPositionWS();
    #if defined(ORTHO_SUPPORT) 
	    float3 cameraForward = UNITY_MATRIX_V[2].xyz;
	    float3 rayStart = ProjectOnPlane(wpos - cameraPosition, cameraForward) + cameraPosition;
        return lerp(cameraPosition, rayStart, unity_OrthoParams.w);
    #elif DF2_BOX_PROJECTION
	    float3 cameraForward = UNITY_MATRIX_V[2].xyz;
	    return ProjectOnPlane(wpos - cameraPosition, cameraForward) + cameraPosition;
    #else
        return cameraPosition;
    #endif
}


inline void GetRawDepth(float2 uv) {
    rawDepth = SampleSceneDepth(DF2_FLIP_DEPTH_TEXTURE ? float2(uv.x, 1.0 - uv.y) : uv);
}


void ClampRayDepth(float3 rayStart, float4 scrPos, inout float t1) {

    float2 uv =  scrPos.xy / scrPos.w;

    // World position reconstruction
    GetRawDepth(uv);

    float depth01 = Linear01Depth(rawDepth, _ZBufferParams);
    if (depth01 > 0.999) return;

    float4 positionCS = float4(uv * 2.0 - 1.0, rawDepth, 1.0);
    float4 raw   = mul(UNITY_MATRIX_I_VP, positionCS);
    float3 worldPos  = raw.xyz / raw.w;
    float z = distance(rayStart, worldPos.xyz);

    #if defined(ORTHO_SUPPORT)
        #if defined(UNITY_REVERSED_Z)
            rawDepth = 1.0 - rawDepth;
        #endif
        z = lerp(z, lerp(_ProjectionParams.y, _ProjectionParams.z, rawDepth), unity_OrthoParams.w);

    #endif
    t1 = min(t1, z);
}


// Alternate reconstruct API; URP 7.4 doesn't set UNITY_MATRIX_I_VP correctly in VR, so we need to use this alternate method

inline float GetLinearEyeDepth(float2 uv) {
    GetRawDepth(uv);
  	float sceneLinearDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
    #if defined(ORTHO_SUPPORT)
        if (unity_OrthoParams.w) {
            #if UNITY_REVERSED_Z
                rawDepth = 1.0 - rawDepth;
            #endif
            float orthoDepth = lerp(_ProjectionParams.y, _ProjectionParams.z, rawDepth);
            sceneLinearDepth = lerp(sceneLinearDepth, orthoDepth, unity_OrthoParams.w);
        }
    #endif
    return sceneLinearDepth;
}


void ClampRayDepthAlt(float3 rayStart, float4 scrPos, inout float t1) {
    float2 uv =  scrPos.xy / scrPos.w;
    float vz = GetLinearEyeDepth(uv);

    #if defined(ORTHO_SUPPORT)
        if (unity_OrthoParams.w) {
            t1 = min(t1, vz);
            return;
        }
    #endif
    float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
    float2 suv = uv;
    #if UNITY_SINGLE_PASS_STEREO
        // If Single-Pass Stereo mode is active, transform the
        // coordinates to get the correct output UV for the current eye.
        float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
        suv = (suv - scaleOffset.zw) / scaleOffset.xy;
    #endif
    float3 vpos = float3((suv * 2 - 1) / p11_22, -1) * vz;
    float4 wpos = mul(unity_CameraToWorld, float4(vpos, 1));
    float z = distance(rayStart, wpos.xyz);
    t1 = min(t1, z);
}

float3 _BoundsCenter, _BoundsExtents;

float BoundsIntersection(float3 origin, float3 viewDir) {
    float3 ro     = origin - _BoundsCenter;
    float3 invR   = 1.0.xxx / viewDir;
    float3 tbot   = invR * (-_BoundsExtents - ro);
    float3 ttop   = invR * (_BoundsExtents - ro);
    float3 tmin   = min (ttop, tbot);
    float2 tt0    = max (tmin.xx, tmin.yz);
    return max(tt0.x, tt0.y);
}

#endif // DYNAMIC_FOG_2_COMMONS_URP

