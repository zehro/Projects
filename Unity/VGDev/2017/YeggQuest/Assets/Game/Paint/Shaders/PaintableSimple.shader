Shader "Paint/PaintableSimple"
{
	Properties
	{
		_Scale("_Scale", Vector) = (1, 1, 1)

		_Color("Color", Color) = (0, 0, 0, 0)
		_ColorPrev("ColorPrev", Color) = (0, 0, 0, 0)
		_LocalPoint("Local Point", Vector) = (0, 0, 0)
		_LocalRadius("Local Radius", Float) = 1
		_EffectSize("Effect Size", Float) = 1
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
		#include "UnityCG.cginc"
		
		// Vertex shader

		struct Input
		{
			float3 localPos;
			float3 localNormal;
		};
		
		float3 _Scale;

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			o.localPos = v.vertex * _Scale;
			o.localNormal = v.normal;
		}

		// Surface shader

		sampler2D _PaintCutoffTex;
		sampler2D _PaintNormalTex;

		float4 _Color;
		float4 _ColorPrev;
		float3 _LocalPoint;
		float _LocalRadius;
		float _EffectSize;

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

			// Where in the transition is this position?
			// Positive outside the radius, negative inside, 0 on the edge
			
			float distortion = (pCutoff - 0.5) * _EffectSize * 0.5;
			float t = distance(_LocalPoint, IN.localPos) - _LocalRadius + distortion;
			float tStep = step(t, 0);

			// First, get the base color based on the transition.
			// If the base color has no alpha, clip this pixel

			float4 result = lerp(_ColorPrev, _Color, tStep);
			clip(result.a - 0.5);

			// Set generic output things

			o.Albedo = result.rgb;
			o.Smoothness = 0.9;
			
			// Now for the special effects - normal and emission ripple along edge

			float fx = (1 - saturate(abs(-t) / _EffectSize));
			float emitStrength = pow(fx + pCutoff *_EffectSize * 0.125, 4);
			float normalStrength = lerp(0.1, 8, pow(fx, 2));

			o.Emission = o.Albedo * 0.2 + emitStrength * tStep;
			o.Normal = normalize(pNormal / float3(1, 1, normalStrength));
		}

		ENDCG
	}

	FallBack "Diffuse"
}