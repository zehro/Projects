Shader "Unlit/CircleWipe"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Wipe ("Wipe", Range(0.0, 1.0)) = 0.5
		_Fuzziness ("Fuzziness", Range(0.0, 0.5)) = 0.1
	}
	
	SubShader
	{
		Tags
		{
			"RenderQueue" = "Overlay"
			"RenderType" = "Transparent"
		}
		
		Blend SrcAlpha OneMinusSrcAlpha
		
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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float _Wipe;
			float _Fuzziness;
			
			fixed4 frag (v2f i) : SV_Target
			{
				float t = clamp(_Wipe,0,1);
				float lowerBias = 0.5 - _Fuzziness;
				float upperBias = 0.5 + _Fuzziness;
				float lowerBound = t / ((1 / lowerBias - 2) * (1 - t) + 1);
				float upperBound = t / ((1 / upperBias - 2) * (1 - t) + 1);
				
				fixed4 col = tex2D(_MainTex, i.uv);
				float value = col.r + col.g * 256.0 + col.b * 256.0 * 256.0;
				value /= 256.0 * 256.0;
				
				//return fixed4(value,0,0,1);
				//return tex2D(_MainTex, i.uv);
				return fixed4(0,0,0,(value - lowerBound)/(upperBound - lowerBound));
			}
			ENDCG
		}
	}
	
	Fallback "Unlit/TransparentCutout"
}
