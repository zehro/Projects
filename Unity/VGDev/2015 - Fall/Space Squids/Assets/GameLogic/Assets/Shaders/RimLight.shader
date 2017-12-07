// This shader is basically a partial recreation
// of what the Standard shader does, except with
// added parameters that allow for rim lighting

Shader "Custom/RimLight"
{
	Properties
	{
		_MainTex ("Albedo", 2D) = "white" {}
		_Color ("Albedo Color", Color) = (1,1,1,1)
		_Metallic ("Metallic", Range(0,1)) = 0
		_Smoothness ("Smoothness", Range(0,1)) = 0
		
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpIntensity ("Normal Intensity", Range(0.01,2)) = 1
		
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimPower ("Rim Power", Range(0.5,8)) = 3
		_RimStrength ("Rim Strength", Range(0,1)) = 1
	}
	
	SubShader
	{
		// Setup
		
		Tags
		{
			"Queue" = "Geometry"
			"RenderType" = "Geometry"
		}
		
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0
		
		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
			fixed4 vertColor : Color;
		};
		
		// Surface shader
		
		sampler2D _MainTex;
		fixed4 _Color;
		half _Metallic;
		half _Smoothness;
		
		sampler2D _BumpMap;
		half _BumpIntensity;
		
		fixed4 _RimColor;
		half _RimPower;
		half _RimStrength;
		
		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 result = tex2D(_MainTex,IN.uv_MainTex)*IN.vertColor*_Color;
			o.Albedo = result.rgb;
			o.Alpha = result.a;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			
			o.Normal = UnpackNormal(tex2D(_BumpMap,IN.uv_BumpMap));
            o.Normal.z /= _BumpIntensity;
            o.Normal = normalize((half3)o.Normal);
            
			half rim = 1.0 - saturate(dot(IN.viewDir,o.Normal));
			o.Emission = _RimColor.rgb*pow(rim,_RimPower)*_RimStrength;
		}
		
		ENDCG
	}
	
	FallBack "Diffuse"
}