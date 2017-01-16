Shader "Custom/Water"
{
	Properties
	{
		_Tint ("Tint", Color) = (1,1,1,1)
		_Smoothness ("Gloss", Range(0, 1)) = 0
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpIntensity ("Normal Intensity", Range(0.01, 2)) = 1
		_BumpSpeed ("Normal Speed", Range(0, 1)) = 1
	}
	
	Subshader
	{
		// Water is in the transparent queue, rendering after geometry
		
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}
		
		// Surface shader
		
		CGPROGRAM
		#pragma surface surf Standard alpha
		#pragma target 3.0
		#include "UnityCG.cginc"
		
		struct Input
		{
			float4 pos;
			float2 uv_BumpMap;
			float4 vertColor : Color;
		};
		
		float4 _Tint;
		half _Smoothness;
		sampler2D _BumpMap;
		half _BumpIntensity;
		half _BumpSpeed;

		void surf(Input i, inout SurfaceOutputStandard o)
		{
			o.Albedo = _Tint;
			o.Alpha = _Tint.a;// *i.vertColor.r;
			o.Smoothness = _Smoothness;// *i.vertColor.r;

			float2 NCoord1 = (i.uv_BumpMap + float2(0.4, -0.2) * _Time.x * _BumpSpeed);
			float2 NCoord2 = (i.uv_BumpMap + float2(-0.8, 0.6) * _Time.x * _BumpSpeed) * 2.5;
			float3 N1 = UnpackNormal(tex2D(_BumpMap, NCoord1)) * 0.5 * i.vertColor.r;
			float3 N2 = UnpackNormal(tex2D(_BumpMap, NCoord2)) * 0.5 * i.vertColor.r;
			o.Normal = normalize((N1 + N2) / float3(1, 1, _BumpIntensity * max(0.001, i.vertColor.r)));
		}
		
		ENDCG
	}
	FallBack "Diffuse"
}