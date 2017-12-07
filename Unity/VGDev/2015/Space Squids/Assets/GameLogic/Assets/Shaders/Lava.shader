Shader "Custom/Lava"
{
	Properties
	{
		_MainTex ("Rocks", 2D) = "white" {}
		_LavaTex ("Lava Map", 2D) = "white" {}
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
			float2 uv_LavaTex;
			float2 uv_BumpMap;
			float3 viewDir;
			fixed4 vertColor : Color;
		};
		
		// Surface shader
		
		sampler2D _MainTex;
		sampler2D _LavaTex;
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
			fixed2 lavaFlowUV = IN.uv_LavaTex + fixed2(_Time.x,_Time.x);
			fixed4 lavaFlow = tex2D(_LavaTex,lavaFlowUV)*IN.vertColor*_Color;
			fixed4 lava = tex2D(_MainTex,IN.uv_MainTex)*IN.vertColor*_Color;
			fixed4 result = lerp(lavaFlow,lava,lava.a);
			o.Albedo = result.rgb;
			o.Alpha = result.a;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			
			o.Normal = UnpackNormal(tex2D(_BumpMap,IN.uv_BumpMap));
            o.Normal.z /= _BumpIntensity;
            o.Normal = normalize((half3)o.Normal);
            o.Normal = lerp(o.Normal, fixed3(0.0, 0.0, 0.0), 1.0 - lava.a);
            
			half rim = 1.0 - saturate(dot(IN.viewDir,o.Normal));
			fixed3 rimLight = _RimColor.rgb*pow(rim,_RimPower)*_RimStrength;
			fixed3 lavaEmit = lavaFlow.rgb;
			o.Emission = lerp(rimLight, lavaEmit, 1.0 - lava.a);
		}
		
		ENDCG
	}
	
	FallBack "Diffuse"
}