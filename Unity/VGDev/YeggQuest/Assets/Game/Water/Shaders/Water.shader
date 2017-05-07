Shader "Water/Water"
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
		};

		sampler2D _NormalTex;

		fixed4 _WaterColor;
		half _WaterSmoothness;

		fixed4 _RimColor;
		half _RimPower;
		half _RimStrength;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float2 uvW = IN.worldPos.xz * 0.25 - IN.worldPos.y * 0.125;
			float2 uv1 = uvW + float2(_Time.x * -13, _Time.x * +4) * 0.25;
			float2 uv2 = uvW * 1.5 + float2(_Time.x *  -8, _Time.x * -7) * 0.25 + float2(0.4, 0.6);

			float camDist = distance(IN.worldPos, _WorldSpaceCameraPos);
			clip(camDist - 1.5);

			o.Albedo = _WaterColor;
			o.Smoothness = _WaterSmoothness;
			o.Alpha = _WaterColor.a;

			float3 n1 = UnpackNormal(tex2D(_NormalTex, uv1)) * 0.5;
			float3 n2 = UnpackNormal(tex2D(_NormalTex, uv2)) * 0.5;
			o.Normal = normalize((n1 + n2) / float3(1, 1, 0.15));

			half rim = 1 - saturate(dot(IN.viewDir, o.Normal));
			o.Emission = max(float3(0.1, 0.1, 0.1), _RimColor.rgb * pow(rim, _RimPower) * _RimStrength);
		}

		ENDCG
	}

	FallBack Off
}