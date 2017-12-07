Shader "ProcBlock/ProcBlock"
{
	Properties
	{
		_AlbedoTex("Albedo", 2D) = "white" {}
		_Color("Diffuse Color", Color) = (1,1,1,1)
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
			float3 localPos;
			float3 localNormal;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			float4x4 m = unity_ObjectToWorld;
			o.localPos = v.vertex;
			o.localPos.x *= length(float3(m[0][0], m[1][0], m[2][0]));
			o.localPos.y *= length(float3(m[0][1], m[1][1], m[2][1]));
			o.localPos.z *= length(float3(m[0][2], m[1][2], m[2][2]));
			o.localNormal = v.normal;
		}

		// Surface shader

		sampler2D _AlbedoTex;
		float4 _Color;
		sampler2D _MetallicTex;
		float _Smoothness;
		sampler2D _NormalTex;
		float _NormalStrength;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half3 wUV = IN.localPos * 0.25;
			half3 wBlend = abs(IN.localNormal);
			wBlend /= dot(wBlend, 1.0);

			float3 aX = tex2D(_AlbedoTex, wUV.yz);
			float3 aY = tex2D(_AlbedoTex, wUV.xz);
			float3 aZ = tex2D(_AlbedoTex, wUV.xy);
			o.Albedo = (aX * wBlend.x + aY * wBlend.y + aZ * wBlend.z) * _Color;

			float mX = tex2D(_MetallicTex, wUV.yz).a;
			float mY = tex2D(_MetallicTex, wUV.xz).a;
			float mZ = tex2D(_MetallicTex, wUV.xy).a;
			o.Smoothness = _Smoothness * (mX * wBlend.x + mY * wBlend.y + mZ * wBlend.z);

			float3 nX = UnpackNormal(tex2D(_NormalTex, wUV.yz));
			float3 nY = UnpackNormal(tex2D(_NormalTex, wUV.xz));
			float3 nZ = UnpackNormal(tex2D(_NormalTex, wUV.xy));
			o.Normal = normalize((nX * wBlend.x + nY * wBlend.y + nZ * wBlend.z) / float3(1, 1, _NormalStrength));
		}

		ENDCG
	}

	FallBack "Diffuse"
}