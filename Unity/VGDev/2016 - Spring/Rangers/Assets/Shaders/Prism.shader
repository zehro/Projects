Shader "Hidden/Prism"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
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

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col1 = tex2D(_MainTex, half2(i.uv.x+0.0005,i.uv.y+0.0005)).r;
				fixed4 col2 = tex2D(_MainTex, half2(i.uv.x-0.0005,i.uv.y-0.0005)).g;
				fixed4 col3 = tex2D(_MainTex, half2(i.uv.x-0.0005,i.uv.y+0.0005)).b;

				// just invert the colors
				return fixed4(col1.x,col2.x,col3.x,1);
			}
			ENDCG
		}
	}
}
