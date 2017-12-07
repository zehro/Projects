Shader "Unlit/Spline"
{
	Properties
	{
		_Speed("Speed", Float) = 1
		_Color("Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
		}

		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			float _Speed;
			fixed4 _Color;

			fixed4 frag (v2f i) : SV_Target
			{
				float t = i.uv.x - _Time.y * _Speed;
				clip(frac(t) - 0.5);

				fixed4 col = _Color;
				UNITY_APPLY_FOG(i.fogCoord, _Color);
				return col;
			}

			ENDCG
		}
	}
}