Shader "Paint/PaintableFull"
{
	Properties
	{
		_Scale("_Scale", Vector) = (1, 1, 1)

		_PaintTex("Paint Texture (Information)", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
		}

		LOD 200
		Offset -1, -1

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow
		#pragma target 3.0

		// Vertex shader

		struct Input
		{
			float3 localPos;
			float3 localNormal;
			float2 paintUV;
		};

		float3 _Scale;

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			o.localPos = v.vertex * _Scale;
			o.localNormal = v.normal;
			o.paintUV = v.texcoord1.xy;
		}

		// Surface shader

		sampler2D _PaintCutoffTex;
		sampler2D _PaintNormalTex;

		sampler2D _PaintTex;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// Get the cutoff (pCutoff) [0-1] and the normal (pNormal) at the current position

			half3 pUV = IN.localPos * 0.25;
			half3 pBlend = abs(IN.localNormal);
			pBlend /= dot(pBlend, 1.0);

			float pcX = tex2D(_PaintCutoffTex, pUV.yz).r;
			float pcY = tex2D(_PaintCutoffTex, pUV.xz).r;
			float pcZ = tex2D(_PaintCutoffTex, pUV.xy).r;
			float pCutoff = (pcX * pBlend.x + pcY * pBlend.y + pcZ * pBlend.z);

			float3 pnX = UnpackNormal(tex2D(_PaintNormalTex, pUV.yz));
			float3 pnY = UnpackNormal(tex2D(_PaintNormalTex, pUV.xz));
			float3 pnZ = UnpackNormal(tex2D(_PaintNormalTex, pUV.xy));
			float3 pNormal = (pnX * pBlend.x + pnY * pBlend.y + pnZ * pBlend.z);

			// Then get the actual paint color by applying some distortion from pCutoff

			float2 pDistortion = float2(0.5 - pCutoff, pCutoff - 0.5) * 0.01;
			float4 paint = tex2D(_PaintTex, IN.paintUV.xy + pDistortion);

			// Final out: clip pixel if alpha is too low

			clip(paint.a - pCutoff);

			o.Albedo = paint.rgb / paint.a;
			o.Smoothness = 0.9;
			o.Emission = o.Albedo * 0.2;
			o.Normal = normalize(pNormal / float3(1, 1, lerp(4, 0.1, paint.a)));
		}

		ENDCG
	}

	FallBack "Diffuse"
}