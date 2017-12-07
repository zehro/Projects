Shader "Water/WaterEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RefractionTex("Refraction Texture", 2D) = "white" {}
		_Strength("Strength", Range(0, 1)) = 0
	}

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _RefractionTex;
			float _Strength;

			fixed4 frag (v2f i) : SV_Target
			{
				// Refraction

				i.uv = lerp(i.uv, (i.uv - 0.5) * 0.95 + 0.5, _Strength);
				
				float2 uv1 = i.uv * 2 + float2(_Time.x * +0.8, _Time.x * -0.6);
				float2 uv2 = i.uv * 3 + float2(_Time.x * -0.4, _Time.x * +1.0);
				float r = (tex2D(_RefractionTex, uv1).r + tex2D(_RefractionTex, uv2).r) * 0.5 - 0.5;
				
				float a = 1 - (i.uv.x) * (1 - i.uv.x) * (i.uv.y) * (1 - i.uv.y) * 16;
				a = pow(a, 3) * _Strength;

				fixed4 col = tex2D(_MainTex, i.uv + r * a * 0.05);

				// Color curves
				
				col.r = lerp(col.r, pow(col.r, 2) + 0.1, _Strength);
				col.g = lerp(col.g, pow(col.g, 0.9), _Strength);
				col.b = lerp(col.b, pow(col.b, 0.5) + 0.1, _Strength);
				
				// Vignetting

				col.r -= a * 0.30;
				col.g -= a * 0.15;
				col.b -= a * 0.25;

				return col;
			}

			ENDCG
		}
	}
}