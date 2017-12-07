Shader "Paint/PaintableBird"
{
	Properties
	{
		_PlaneOrigin("Plane Origin", Vector) = (0, 0, 0)
		_PlaneNormal("Plane Normal", Vector) = (0, 1, 0)
		_PlaneHeight("Plane Height", Float) = 0

		_SurfaceMaskTex("Surface Mask Texture", 2D) = "white" {}
		_SurfaceNormalTex("Surface Normal Texture", 2D) = "white" {}
		_SurfaceOcclusionTex("Surface Occlusion Texture", 2D) = "white" {}
		_Color("Color", Color) = (0, 0, 0, 0)
		_ColorPrev("ColorPrev", Color) = (0, 0, 0, 0)
		_ColorTransition("ColorTransition", Range(0,1)) = 0
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
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 3.0

		// Vertex shader

		struct Input
		{
			float planePos;
			float3 localPos;
			float3 localNormal;
			float2 uv_SurfaceMaskTex;
		};

		float3 _PlaneOrigin;
		float3 _PlaneNormal;
		float _PlaneHeight;

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			o.planePos = dot(_PlaneOrigin, _PlaneNormal) - dot(worldPos, _PlaneNormal) + _PlaneHeight;
			o.localPos = v.vertex;
			o.localNormal = v.normal;
		}

		// Surface shader

		sampler2D _PaintCutoffTex;
		sampler2D _PaintNormalTex;

		sampler2D _SurfaceMaskTex;
		sampler2D _SurfaceNormalTex;
		sampler2D _SurfaceOcclusionTex;
		float4 _Color;
		float4 _ColorPrev;
		float _ColorTransition;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// Get the cutoff (pCutoff) [0-1] and the normal (pNormal) at the current position

			half3 pUV = IN.localPos;
			pUV.x -= _Time.x * 1.5;
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
			float3 surfaceNormal = UnpackNormal(tex2D(_SurfaceNormalTex, IN.uv_SurfaceMaskTex));

			// Where in the transition is this position?

			float t = _ColorTransition - 0.5 + pCutoff - 0.5;
			float tStep = step(t, 0);

			// Get the base color based on the transition.
			// If it has no alpha, clip this pixel

			float4 result = lerp(_Color, _ColorPrev, tStep);
			clip(result.a - 0.5);

			// Also clip based on the paint plane and mask

			clip(IN.planePos - pCutoff * 0.05);
			clip(tex2D(_SurfaceMaskTex, IN.uv_SurfaceMaskTex).a - 0.5);
			
			// Set generic output things

			o.Albedo = result.rgb * tex2D(_SurfaceOcclusionTex, IN.uv_SurfaceMaskTex);
			o.Smoothness = 0.9;
			o.Emission = o.Albedo * 0.2;

			// Special effect: bump the normal up high along the edge of the paint

			float n = saturate(abs(IN.planePos * 10) * abs(IN.planePos * 10));
			n *= saturate(abs(t) * 10);
			float normalStrength = lerp(8, 0.5, n);
			o.Normal = normalize((pNormal + surfaceNormal) / float3(1, 1, normalStrength));
		}

		ENDCG
	}

	FallBack "Diffuse"
}