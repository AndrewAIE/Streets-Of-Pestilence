Shader "DynamicFog2/DynamicFog2DURP"
{
	Properties
	{
		[HideInInspector] _MainTex("Noise Texture", 2D) = "white" {}
		[HideInInspector] _NoiseData("Noise Data", Vector) = (0.005, 15, 0)
		[HideInInspector] _Color("Color", Color) = (1,1,1)
		[HideInInspector] _SecondColor("Second Color", Color) = (1,1,1)
		[HideInInspector] _Density("Density", Vector) = (1,1,1)
		[HideInInspector] _DensityData("Density Data", Vector) = (1,1,1)
		[HideInInspector] _LightColor ("Directional Light Color", Color) = (1,1,1)
		[HideInInspector] _LightDiffusionPower("Sun Diffusion Power", Range(1, 64)) = 32
		[HideInInspector] _LightDiffusionIntensity("Sun Diffusion Intensity", Range(0, 1)) = 0.4
		[HideInInspector] _WindDirection("Wind Direction", Vector) = (1, 0, 0)
		[HideInInspector] _DitherStrength("Dither Strength", Range(0, 2)) = 1.0
		[HideInInspector] _SunDir("Sun Direction", Vector) = (1,0,0)
		[HideInInspector] _FogOfWarTex("Fog Of War", 2D) = "white" {}
		[HideInInspector] _FogOfWarCenter("Fog Of War Center", Vector) = (0,0,0)
		[HideInInspector] _FogOfWarSize("Fog Of War Size", Vector) = (1024,0,1024)
		[HideInInspector] _FogOfWarCenterAdjusted("Fog Of War Center Adjusted", Vector) = (0,0,0)
		[HideInInspector] _Geom("Geometry", Vector) = (0, 60, 0, 1)
		[HideInInspector] _GradientTex("Gradient Texture", 2D) = "white" {}
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent+100" "DisableBatching" = "True" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Always
			Cull Front
			ZWrite Off

			Pass
			{
				Tags { "LightMode" = "UniversalForward" }
				HLSLPROGRAM
				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x
				#pragma target 3.0
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_local _ DF2_FOW
				#pragma multi_compile_local _ DF2_BOX_PROJECTION
				#pragma multi_compile_local _ DF2_DEPTH_CLIP
				#pragma multi_compile_local _ DF2_3D_DISTANCE

				#include "Fog.hlsl"

				struct appdata
				{
					float4 vertex : POSITION;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f
				{
					float4 pos     : SV_POSITION;
                    float3 wpos    : TEXCOORD0;
					float4 scrPos  : TEXCOORD1;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
				};

				v2f vert(appdata v)
				{
					v2f o;

					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

					o.pos = TransformObjectToHClip(v.vertex.xyz);
				    o.wpos = TransformObjectToWorld(v.vertex.xyz);
					o.scrPos = ComputeScreenPos(o.pos);

					#if defined(UNITY_REVERSED_Z)
						o.pos.z = o.pos.w * UNITY_NEAR_CLIP_VALUE * 0.99999; //  0.99999 avoids precision issues on some Android devices causing unexpected clipping of light mesh
					#else
						o.pos.z = o.pos.w - 1.0e-6f;
					#endif

					return o;
				}


				half4 frag(v2f i) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID(i);
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

					float3 rayStart = GetRayStart(i.wpos);
					float3 ray = i.wpos - rayStart;
                   	float t1 = length(ray);
					float3 rayDir = ray / t1;

					CLAMP_RAY_DEPTH(rayStart, i.scrPos, t1);

					#if DF2_DEPTH_CLIP
						float t0 = BoundsIntersection(rayStart, rayDir);
						clip(t1-t0);
					#endif

					SetJitter(i.scrPos);

					return GetFogColor(rayStart, rayDir, t1);
				}
				ENDHLSL
			}

		}
}
