Shader "Water/RunningWaterFoam"
{
	Properties
	{
		_CutoffTex("Cutoff Texture", 2D) = "white" {}
		_NormalTex("Normal Texture", 2D) = "white" {}

		_FoamColor("Foam Color", Color) = (1,1,1,1)
		_FoamSmoothness("Foam Smoothness", Range(0, 1)) = 0.6
		_FoamFalloff("Foam Falloff", Range(0.25, 4)) = 1
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower("Rim Power", Range(0.5, 8)) = 3
		_RimStrength("Rim Strength", Range(0, 1)) = 1
	}

	SubShader
	{
		Cull Off

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows addshadow
		#pragma target 3.0
		
		struct Input
		{
			float2 uv_NormalTex;
			float3 viewDir;
			float3 worldPos;
			float4 vertColor : Color;
		};

		sampler2D _CutoffTex;
		sampler2D _NormalTex;

		float4 _BirdPosition;

		fixed4 _FoamColor;
		float _FoamSmoothness;
		float _FoamFalloff;

		fixed4 _RimColor;
		half _RimPower;
		half _RimStrength;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float2 uvW = IN.uv_NormalTex;
			float2 uv1 = uvW       + float2(_Time.x * -13, 0) * 0.5;
			float2 uv2 = uvW * 1.5 + float2(_Time.x *  -8, 0) * 0.5 + float2(0.4, 0.6);
			float foam = lerp(tex2D(_CutoffTex, uv1).r, tex2D(_CutoffTex, uv2).r, 0.25);

			float2 foamUV = 0;
			foamUV.x = IN.worldPos.x * 0.4 + IN.worldPos.y * 0.8 + IN.worldPos.z * 0.6;
			foamUV.y = IN.worldPos.x * 0.6 + IN.worldPos.y * 0.9 + IN.worldPos.z * 0.4;
			foamUV = foamUV * 0.025 + _Time.x * 0.5;
			foam += (tex2D(_CutoffTex, foamUV).r - 0.5) * 0.3;

			float camDist = distance(IN.worldPos, _WorldSpaceCameraPos);
			float camFoam = max(0, 2 - camDist);
			foam += camFoam;
			clip(1 - foam - IN.vertColor.g);

			float birdDist = distance(IN.worldPos, _BirdPosition);
			float birdFoam = max(0, 0.65 - birdDist);
			foam += birdFoam * 1.5;

			foam = step(pow(IN.vertColor.r, _FoamFalloff), foam);
			
			clip(foam - 0.5);

			o.Albedo = _FoamColor;
			o.Smoothness = _FoamSmoothness;
			o.Alpha = foam;

			float3 n1 = UnpackNormal(tex2D(_NormalTex, uv1)) * 0.5;
			float3 n2 = UnpackNormal(tex2D(_NormalTex, uv2)) * 0.5;
			o.Normal = normalize(lerp(n1, n2, 0.25) / float3(1, 1, 0.25));

			half rim = 1 - saturate(dot(IN.viewDir, o.Normal));
			o.Emission = max(float3(0.1, 0.1, 0.1), _RimColor.rgb * pow(rim, _RimPower) * _RimStrength);
		}

		ENDCG
	}

	FallBack "Diffuse"
}