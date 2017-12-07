Shader "Custom/TreeLeaves"
{
	Properties
	{
		_MainColor("Main Color", Color) = (1, 1, 1, 1)
		_StripeColor("Stripe Color", Color) = (1, 1, 1, 1)

		_CutoffTex("Cutoff", 2D) = "white" {}
		_MetallicTex("Metallic", 2D) = "white" {}
		_Smoothness("Smoothness", Range(0,1)) = 0
		_NormalTex("Normal", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Range(0.01,2)) = 1

		_WavePeriod("Wave Period", Range(0.01, 20)) = 1
		_WaveStrength("Wave Strength", Range(0.01, 2)) = 1
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
		}

		LOD 200
		Cull Off

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow
		#pragma target 3.0

		// Vertex shader

		struct Input
		{
			float3 wPos;
			float2 uv_CutoffTex;
			float3 vertColor : COLOR;
		};
		
		float _WavePeriod;
		float _WaveStrength;

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			o.wPos = mul(unity_ObjectToWorld, v.vertex);
			o.vertColor = v.color;

			float pow = v.texcoord.y * v.texcoord.y;
			v.vertex.x += sin(_Time.x * 50 + v.vertex.x * 1.6 * _WavePeriod) * pow * 0.05 * _WaveStrength;
			v.vertex.y += sin(_Time.x * 100 + v.vertex.x * 2.8 * _WavePeriod + v.vertex.z * 1.6 * _WavePeriod) * pow * 0.0125 * _WaveStrength;
		}

		// Surface shader

		fixed4 _MainColor;
		fixed4 _StripeColor;

		sampler2D _CutoffTex;
		sampler2D _MetallicTex;
		float _Smoothness;
		sampler2D _NormalTex;
		float _NormalStrength;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float2 cutoffUV = IN.uv_CutoffTex;

			clip(tex2D(_CutoffTex, cutoffUV).r - 0.15);

			float4 aColor = lerp(_MainColor, _StripeColor, 1 - IN.vertColor.r);
			float4 cColor = tex2D(_CutoffTex, cutoffUV);

			float4 shadowCol = float4(0.25, 0.20, 0.05, 0);

			o.Albedo = aColor - (1 - cColor.r) * shadowCol;
			o.Albedo -= (1 - IN.uv_CutoffTex.y) * shadowCol;
			o.Emission = o.Albedo * 0.1;

			o.Smoothness = _Smoothness * tex2D(_MetallicTex, cutoffUV).r;

			float3 norm = UnpackNormal(tex2D(_NormalTex, cutoffUV));
			o.Normal = normalize(norm / float3(1, 1, _NormalStrength * pow(IN.uv_CutoffTex.y, 4)));
		}

		ENDCG
	}

	FallBack "Diffuse"
}