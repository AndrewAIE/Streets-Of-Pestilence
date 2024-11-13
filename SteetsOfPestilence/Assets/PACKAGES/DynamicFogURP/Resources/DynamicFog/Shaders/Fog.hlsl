#include "CommonsURP.hlsl"
#include "FogOfWar.hlsl"

sampler2D _MainTex;
sampler2D _GradientTex;
float jitter;
half4 _Color;
half4 _SecondColor;

float3 _SunDir;
float _LightDiffusionIntensity, _LightDiffusionPower;
float3 _WindDirection;
float _DitherStrength;
half3 _LightColor;
float3  _DensityData;
float4 _Geom;
float4 _NoiseData;

#define NOISE_SCALE _NoiseData.x
#define NOISE_INTENSITY _NoiseData.y
#define NOISE_SHIFT _NoiseData.z
#define NOISE_COLOR _NoiseData.w
#define NOISE_DISTANCE_ATTEN _Geom.w

#define BOTTOM_LEVEL _Geom.x
#define MAX_HEIGHT _Geom.y
#define DISTANCE_MAX _Geom.z

float3  _Density;
#define DENSITY _Density.x
#define HEIGHT_FALLOFF _Density.y

void SetJitter(float4 scrPos) {
    float2 uv = (scrPos.xy / scrPos.w) * _ScreenParams.xy;
    const float3 magic = float3( 0.06711056, 0.00583715, 52.9829189 );
    jitter = frac( magic.z * frac( dot( uv, magic.xy ) ) );
}

half4 GetFogColor(float3 rayStart, float3 rayDir, float t1) {

    clip(DISTANCE_MAX - t1);

    float3 wpos = rayStart + rayDir * t1;

    #if UNITY_REVERSED_Z
        rawDepth = 1.0 - rawDepth;
    #endif

	half sunAmount = max( dot( rayDir, _SunDir.xyz ), 0.0 );    
	half diffusion = step(0.99999, rawDepth) * pow(sunAmount, _LightDiffusionPower) * _LightDiffusionIntensity;
    float t = (abs(_SunDir.y) + abs(rayDir.y)) * 0.5;
    half4 gradientColor = tex2D(_GradientTex, float2(t, 0));

    float2 noiseTexCoord = wpos.xz * NOISE_SCALE + _WindDirection.xz;
    half noise = tex2D(_MainTex, noiseTexCoord).r;
    half colorNoise = saturate(noise + NOISE_SHIFT) * NOISE_COLOR;
    colorNoise *= 1.0 - rawDepth * NOISE_DISTANCE_ATTEN;

    half4 baseColor = lerp(_Color, _SecondColor, colorNoise);
	half3 fogColor = baseColor.rgb * gradientColor.rgb * _LightColor + diffusion;
   
    float rayStartY = rayStart.y - BOTTOM_LEVEL + noise * NOISE_INTENSITY;

    #if DF2_3D_DISTANCE
        rayDir.y = max(0.001, abs(rayDir.y)) * sign(rayDir.y); // prevents division by 0
	    half fogAmount = (HEIGHT_FALLOFF / DENSITY) * exp(-rayStartY * DENSITY) * (1.0-exp( -t1 * rayDir.y * DENSITY )) / rayDir.y;
    #else
        float3 xzPos = wpos;
        xzPos.y = rayStart.y;
        t1 = distance(rayStart, xzPos);
        half fogAmount = (HEIGHT_FALLOFF / DENSITY) * (1.0-exp( -t1 * DENSITY ));
    #endif

    float x = MAX_HEIGHT / max(0.001, wpos.y);
    fogAmount = saturate(fogAmount) * saturate(x);

	half4 res = half4(fogColor, fogAmount * baseColor.a * gradientColor.a);

    #if DF2_FOW
        res *= ApplyFogOfWar(wpos);
    #endif

	res = max(0, res + (jitter - 0.5) * _DitherStrength);

    return res;

}