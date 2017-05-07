Shader "Water/RunningWater"
{
	Properties
	{
		_NormalTex("Normal Texture", 2D) = "white" {}

		_WaterColor("Water Color", Color) = (1, 1, 1, 1)
		_WaterSmoothness("Water Smoothness", Range(0, 1)) = 0.85

		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower("Rim Power", Range(0.5, 8)) = 3
		_RimStrength("Rim Strength", Range(0, 1)) = 1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Overlay"
			"RenderType" = "Transparent"
		}

		Cull Off

		CGPROGRAM
		#pragma surface surf Standard alpha fullforwardshadows
		#pragma target 3.0

		struct Input
		{
			float3 viewDir;
			float3 worldPos;
			float2 uv_NormalTex;
			float4 vertColor : Color;
		};

		sampler2D _NormalTex;

		fixed4 _WaterColor;
		half _WaterSmoothness;

		fixed4 _RimColor;
		half _RimPower;
		half _RimStrength;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float2 uvW = IN.uv_NormalTex;
			float2 uv1 = uvW       + float2(_Time.x * -13, 0) * 0.5;
			float2 uv2 = uvW * 1.5 + float2(_Time.x *  -8, 0) * 0.5 + float2(0.4, 0.6);

			float camDist = distance(IN.worldPos, _WorldSpaceCameraPos);
			clip(camDist - 1.5);
			clip(0.1 - IN.vertColor.g);

			o.Albedo = _WaterColor;
			o.Smoothness = _WaterSmoothness;
			o.Alpha = _WaterColor.a;

			float3 n1 = UnpackNormal(tex2D(_NormalTex, uv1)) * 0.5;
			float3 n2 = UnpackNormal(tex2D(_NormalTex, uv2)) * 0.5;
			o.Normal = normalize(lerp(n1, n2, 0.25) / float3(1, 1, 0.5));

			half rim = 1 - saturate(dot(IN.viewDir, o.Normal));
			o.Emission = max(float3(0.1, 0.1, 0.1), _RimColor.rgb * pow(rim, _RimPower) * _RimStrength);
		}

		ENDCG
	}

	FallBack Off
}