Shader "Custom/PortalDoorSurface"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos       : POSITION;
				float4 screenPos : TEXCOORD0;
			};
			
			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.screenPos = ComputeScreenPos(o.pos);
				return o;
			}

			sampler2D _MainTex;
			float4 _FadeColor;
			
			float4 frag(v2f i) : COLOR
			{
				float4 c = tex2D(_MainTex, i.screenPos.xy / i.screenPos.w);
				c = lerp(c, _FadeColor, _FadeColor.a);
				return c;
			}

			ENDCG
		}
	}
}