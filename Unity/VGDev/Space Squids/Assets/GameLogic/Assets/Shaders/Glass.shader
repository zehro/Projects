// This shader is a derivative of the custom rim light shader
// which is transparent (used for the dome glass)

Shader "Custom/Glass"
{
	Properties
	{
		_Color ("Tint", Color) = (1,1,1,1)
		_Smoothness ("Gloss", Range(0,1)) = 0
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpIntensity ("Normal Intensity", Range(0.01,2)) = 1
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimPower ("Rim Power", Range(0.5,8)) = 3
		_RimStrength ("Rim Strength", Range(0,1)) = 1
	}
	
	Subshader
	{
		// Water is in the transparent queue, rendering after geometry
		// This is necessary so that it can refract things properly
		
		Tags
		{
			"Queue" = "Overlay"
			"RenderType" = "Transparent"
		}
		
		CGPROGRAM
		#pragma surface surf Standard alpha
		#pragma target 3.0
		#include "UnityCG.cginc"
		
		struct Input
		{
			float4 pos;
			float2 uv_BumpMap;
			float3 viewDir;
			float4 vertColor : Color;
		};
		
		fixed4 _Color;
		half _Smoothness;
		sampler2D _BumpMap;
		half _BumpIntensity;
		fixed4 _RimColor;
		half _RimPower;
		half _RimStrength;

		void surf(Input i, inout SurfaceOutputStandard o)
		{
			o.Smoothness = _Smoothness*i.vertColor.r;
			
			o.Normal = UnpackNormal(tex2D(_BumpMap,i.uv_BumpMap));
            o.Normal.z /= _BumpIntensity;
            o.Normal = normalize((half3)o.Normal);
			
			half rim = 1-saturate(dot(i.viewDir,o.Normal));
			o.Emission = max(float3(0.1,0.1,0.1),_RimColor.rgb*pow(rim*i.vertColor.r,_RimPower)*_RimStrength);
		}
		
		ENDCG
	}
	
	FallBack "Diffuse"
}