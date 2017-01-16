Shader "Custom/LogicCable"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};

		float3 _Data;
		float _Fade;
		float _Flash;

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float2 uv = IN.uv_MainTex;

			o.Albedo = tex2D(_MainTex, uv);
			o.Smoothness = 0.75 * o.Albedo;
			o.Emission = step(float3(uv.x, uv.x, uv.x), _Data) * o.Albedo * 2;
			o.Emission *= (1 - 1 * uv.x * _Fade);
			o.Emission = lerp(o.Emission, float3(2.0, 2.0, 2.0), _Flash);
			o.Albedo *= 0.2;
		}
		ENDCG
	}

	FallBack "Diffuse"
}