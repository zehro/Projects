Shader "ProcBlock/ProcBlockGrassTop"
{
	Properties
	{
		_AlbedoTex("Albedo", 2D) = "white" {}
		_CutoffTex("Cutoff", 2D) = "white" {}
		_MetallicTex("Metallic", 2D) = "white" {}
		_Smoothness("Smoothness", Range(0,1)) = 0
		_NormalTex("Normal", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Range(0.01,2)) = 1
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
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.wPos = mul(unity_ObjectToWorld, v.vertex);
		}

		// Surface shader

		sampler2D _AlbedoTex;
		sampler2D _CutoffTex;
		sampler2D _MetallicTex;
		float _Smoothness;
		sampler2D _NormalTex;
		float _NormalStrength;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half3 w = IN.wPos * 1;
			float2 cutoffUV = float2(w.x + w.z, IN.uv_CutoffTex.y);

			float2 albedoUV = w.xz * 0.125;

			clip(tex2D(_CutoffTex, cutoffUV).r - 0.05);

			float4 aColor = tex2D(_AlbedoTex, albedoUV);
			float4 cColor = tex2D(_CutoffTex, cutoffUV);

			o.Albedo = aColor - ((1 - cColor) * float4(0.5, 0.5, 0, 0));
			o.Smoothness = _Smoothness;
			float3 norm = UnpackNormal(tex2D(_NormalTex, cutoffUV));
			o.Normal = normalize(norm / float3(1, 1, _NormalStrength));
		}

		ENDCG
	}

	FallBack "Diffuse"
}