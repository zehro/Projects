Shader "Paint/PaintPool"
{
	Properties
	{
		_Flow ("Flow", Range(0,1)) = 1
		_Color ("Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
		}

		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow
		#pragma target 3.0

		sampler2D _PaintCutoffTex;
		sampler2D _PaintNormalTex;

		struct Input
		{
			float2 paintUV;
		};
		
		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			fixed4 uv = v.texcoord;
			uv.x += _Time.x * 0.125;
			uv.y = pow(uv.y, 0.25);
			uv.y += _Time.x * 5;

			float off = tex2Dlod(_PaintCutoffTex, uv).r;
			v.vertex.z += off * v.texcoord.y * 0.25;

			o.paintUV = v.texcoord;
		}

		float _Flow;
		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float2 uv = IN.paintUV;
			uv.x += _Time.x * 0.125;
			uv.y = pow(uv.y, 0.25);
			uv.y += _Time.x * 5;

			float cutoff = tex2D(_PaintCutoffTex, uv).r * 0.15;
			clip(IN.paintUV.y - cutoff - (1 - _Flow));

			float3 n = UnpackNormal(tex2D(_PaintNormalTex, uv));

			o.Albedo = _Color - cutoff * (1 - pow(IN.paintUV.y, 0.125)) * 8;
			o.Emission = o.Albedo * 0.2;
			o.Normal = normalize(n / float3(1, 1, 0.5 + pow(1 - IN.paintUV.y, 16) * 8));
			o.Smoothness = 0.9;
		}

		ENDCG
	}

	FallBack "Diffuse"
}