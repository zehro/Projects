Shader "Custom/BoostPadScreen"
{
	Properties
	{
		_Strength("Strength", Float) = 1
		_AnimTime("Animation Time", Float) = 0
		_MetallicTex("Metallic Tex", 2D) = "white" {}
		_EmissionTex("Emission Tex", 2D) = "white" {}
		_EmissionColor1("Emission Color 1", Color) = (1,1,1,1)
		_EmissionColor2("Emission Color 2", Color) = (1,1,1,1)
		_NormalTex("Normal Tex", 2D) = "white" {}
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

		struct Input
		{
			float3 localPos;
			float width;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			float4x4 m = unity_ObjectToWorld;
			o.localPos = v.vertex;
			o.width = length(float3(m[0][0], m[1][0], m[2][0]));

			o.localPos.x *= o.width;
			o.localPos.y *= length(float3(m[0][1], m[1][1], m[2][1]));
			o.localPos.z *= length(float3(m[0][2], m[1][2], m[2][2]));
		}

		float _Strength;
		float _AnimTime;
		sampler2D _MetallicTex;
		sampler2D _EmissionTex;
		fixed4 _EmissionColor1;
		fixed4 _EmissionColor2;
		sampler2D _NormalTex;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float2 uv = IN.localPos.xz * 0.5;
			uv.x += 0.5 + 0.03125;
			uv.x += fmod(round(IN.width * 0.5), 2) * 0.5;

			float metallic = tex2D(_MetallicTex, uv);
			o.Albedo = float3(0.1, 0.2, 0.3) + metallic * 0.1;
			o.Smoothness = 0.75 - metallic * 0.2;
			o.Metallic = 1;

			float editorMove = step(_AnimTime, 0) * _Time.z;

			float2 emitUV = floor(uv * 16) / 16;
			float emitOff = abs(0.5 - frac(emitUV.x));
			float t = frac(emitUV.y - emitOff - _AnimTime - editorMove);
			
			float emit = step(1 - t, 0.5);
			fixed4 emitColor = lerp(_EmissionColor1, _EmissionColor2, (1 - t) * 2);
			o.Emission = tex2D(_EmissionTex, uv) * emitColor * emit * _Strength;
			o.Emission += _EmissionColor2 * _Strength * 0.5;

			float3 norm = UnpackNormal(tex2D(_NormalTex, uv));
			o.Normal = normalize(norm / float3(1, 1, 0.04));
		}

		ENDCG
	}

	FallBack "Diffuse"
}