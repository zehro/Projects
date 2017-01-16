// This is an extremely specific use shader which makes palm trees wavy in the wind
// Don't use on literally anything else, it's more or less a dumb vertex shader hack

Shader "Custom/PalmTree"
{
	Properties
	{
		_MainTex ("Albedo", 2D) = "white" {}
		_Color ("Albedo Color", Color) = (1,1,1,1)
		_Cutoff ("Alpha Cutoff", Range(0,1)) = 0.15
		_Metallic ("Metallic", Range(0,1)) = 0
		_Smoothness ("Smoothness", Range(0,1)) = 0
		
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpIntensity ("Normal Intensity", Range(0.01,2)) = 1
		
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimPower ("Rim Power", Range(0.5,8)) = 3
		_RimStrength ("Rim Strength", Range(0,1)) = 1
		
		_WaveSize ("Wave Turbulence",Range(1,100)) = 20
		_WaveStrength ("Wave Amplitude",Range(0,2)) = 1
		_WaveSpeed ("Wave Speed",Range(0,1)) = 1
	}
	
	SubShader
	{
		Tags
		{
			"Queue" = "AlphaTest"
			//"IgnoreProjector" = "True"
			"RenderType" = "TransparentCutout"
		}
		
		// Surface shader
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert alphatest:_Cutoff
		#pragma target 3.0
		#include "UnityCG.cginc"
		
		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir;
			fixed4 vertColor : Color;
		};
		
		float _WaveSize;
		float _WaveStrength;
		float _WaveSpeed;
		
		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			
			float4 s = v.vertex;
			float t = _Time.z*_WaveSpeed;
			float a = 0;
			if (v.texcoord.x > 0.5)
				a = _WaveStrength/2*(1-v.texcoord.y);
			
			s.x += sin((s.x+s.y)*0.01*_WaveSize*1.125+t)*a;
			s.y += sin((s.y+s.z)*0.01*_WaveSize*1.000+t)*a;
			s.z += sin((s.z+s.x)*0.01*_WaveSize*1.250+t)*a;
			
			v.vertex = s;
		}
		
		sampler2D _MainTex;
		fixed4 _Color;
		//float _Cutoff;
		half _Metallic;
		half _Smoothness;
		
		sampler2D _BumpMap;
		half _BumpIntensity;
		
		fixed4 _RimColor;
		half _RimPower;
		half _RimStrength;

		void surf(Input i, inout SurfaceOutputStandard o)
		{
			fixed4 result = tex2D(_MainTex,i.uv_MainTex)*i.vertColor*_Color;
			o.Albedo = result.rgb;
			o.Alpha = result.a;
			o.Metallic = _Metallic;
			if (i.uv_MainTex.x < 0.5)
				o.Smoothness = _Smoothness;
			
			o.Normal = UnpackNormal(tex2D(_BumpMap,i.uv_BumpMap));
            o.Normal.z /= _BumpIntensity;
            o.Normal = normalize((half3)o.Normal);
			
			half rim = 1-saturate(dot(i.viewDir,o.Normal));
			//if (i.uv_MainTex.x < 0.5)
				o.Emission = max(float3(0.1,0.1,0.1),_RimColor.rgb*pow(rim,_RimPower)*_RimStrength)*o.Alpha;
		}
			
		ENDCG
	}
	
	FallBack "Transparent/Cutout/Diffuse"
}