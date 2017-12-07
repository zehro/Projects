Shader "Custom/PostProcessing"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "" {}
	}
	
	Subshader
	{
		Pass
		{
			CGINCLUDE
			#include "UnityCG.cginc"
			#pragma target 3.0

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// Vertex shader

			v2f vert(appdata_img v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}

			// Fragment shader
			
			sampler2D _MainTex;
			sampler2D _GlitchTex;
			float _FXTime;
			float _FXDesync;
			float2 intensity;

			float4 frag(v2f i) : COLOR
			{
				float2 coords = i.uv;
				coords = (coords - 0.5) * 2.0;

				float s1 = tex2D(_GlitchTex, i.uv + float2(1, _FXTime * -5)).r;
				float s2 = tex2D(_GlitchTex, i.uv * float2(0.2, 0.1) + float2(s1, _FXTime * 3)).r;
				float s3 = tex2D(_GlitchTex, i.uv * float2(1, 0.4) + float2(s2, s1)).r;
				float s = s1 * s1 * s2 + s3 + 1;
				s *= _FXDesync;

				float2 realCoordOffs;
				realCoordOffs.x = (1 - coords.y * coords.y) * intensity.y * (coords.x);
				realCoordOffs.y = (1 - coords.x * coords.x) * intensity.x * (coords.y);

				realCoordOffs.x += tex2D(_MainTex, i.uv - realCoordOffs).b * 0.05 * s;
				realCoordOffs.y += tex2D(_MainTex, i.uv - realCoordOffs).g * 0.10 * s;
				realCoordOffs.x -= tex2D(_MainTex, i.uv - realCoordOffs).r * 0.05 * s;

				float r = tex2D(_MainTex, frac(i.uv - realCoordOffs - float2(0.005 * s, 0))).r;
				float g = tex2D(_MainTex, frac(i.uv - realCoordOffs)).g;
				float b = tex2D(_MainTex, frac(i.uv - realCoordOffs + float2(0.005 * s, 0))).b;

				//r += s1 * 0.05f;
				//g += s2 * 0.05f;
				//b += s3 * 0.05f;

				return float4(r, g, b, 1);
			}

			ENDCG
		}

		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}

	Fallback "Diffuse"
}