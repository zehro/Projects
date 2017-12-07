Shader "ProcBlock/ProcBlockGrass"
{
	Properties
	{
		_Scale("Scale", Range(0.01, 2)) = 0.5
		_AlbedoTex("Albedo", 2D) = "white" {}
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

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.0

		// Vertex shader

		struct Input
		{
			float3 wPos;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.wPos = mul(unity_ObjectToWorld, v.vertex);
		}

		// Surface shader

		float _Scale;
		sampler2D _AlbedoTex;
		sampler2D _MetallicTex;
		float _Smoothness;
		sampler2D _NormalTex;
		float _NormalStrength;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half3 w = IN.wPos * _Scale;
			float3 dist = normalize(float3(cos(w.x), sin(w.y * 2), cos(w.z * 1.5))) * 0.01;
			float3 xNorm = float3(0.5, 0, 0.5) + dist;
			float3 yNorm = float3(0.1, 0.5, 0.2) + dist;
			float2 uv = float2(dot(w, xNorm), dot(w, yNorm));

			o.Albedo = tex2D(_AlbedoTex, uv);
			o.Smoothness = tex2D(_MetallicTex, uv).a * _Smoothness;
			o.Emission = o.Albedo * 0.15;
			float3 norm = UnpackNormal(tex2D(_NormalTex, uv));
			o.Normal = normalize(norm / float3(1, 1, _NormalStrength));
		}

		ENDCG
	}

	FallBack "Diffuse"
}