// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Scanner" {
	Properties {
		_Color ("Color", Color) = (1, 0, 0, 0.5)
		_InitialFalloff ("Initial Falloff", Float) = 1
		_NoiseDensity ("Noise Density", Float) = 2
		_NoiseScale ("Noise Scale", Float) = 1
		_BeamColor ("Beam Color", Color) = (1, 0, 0, 0.5)
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200

		CGINCLUDE
		#include "UnityCG.cginc"
		ENDCG
		
		Pass
		{
			Blend One One
			ZWrite Off
			ZTest LEqual
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "noiseSimplex.cginc"

			fixed4 _Color;
			half _NoiseDensity;
			half _NoiseScale;
			half _InitialFalloff;

			struct v2f
			{
				float4 pos : SV_POSITION;
				fixed3 wpos : TEXCOORD0;
				fixed3 lpos : TEXCOORD1;
			};

			v2f vert(appdata_base vtx) {
				v2f result;
				result.lpos = vtx.vertex;
				result.pos = UnityObjectToClipPos(vtx.vertex);
				result.wpos = mul(unity_ObjectToWorld, vtx.vertex) * _NoiseDensity;
				result.wpos.y += _Time.x;
				return result;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float n = sin(snoise(IN.wpos) * _NoiseScale) * 0.5 + 0.5;
				float basis = saturate(IN.lpos.z / _InitialFalloff);
				return fixed4(_Color.rgb * _Color.a * lerp(2, n, basis), 0);
			}

			ENDCG
		}

		Pass
		{
			Blend One One
			ZWrite Off
			ZTest LEqual
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			fixed4 _BeamColor;

			float4 vert(appdata_base vtx) : SV_POSITION {
				return UnityObjectToClipPos(vtx.vertex);
			}

			fixed4 frag() : SV_Target
			{
				return fixed4(_BeamColor.rgb * _BeamColor.a, 0);
			}

			ENDCG
		}
	} 
}
