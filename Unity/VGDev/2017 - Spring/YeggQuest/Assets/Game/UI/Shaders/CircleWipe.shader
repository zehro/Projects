Shader "Custom/CircleWipe"
{
	Properties
	{
		_Color("Diffuse Color", Color) = (1, 1, 1, 1)
		_Center("Center", Vector) = (0.5, 0.5, 0, 0)
		_Wipe("Wipe", Range(0, 1)) = 1
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
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 screenPos : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 screenPos : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				float4 clipSpace = mul(UNITY_MATRIX_MVP, v.vertex);
				clipSpace.xy /= clipSpace.w;
				clipSpace.xy = 0.5 * clipSpace.xy + 0.5;
				o.screenPos = clipSpace.xy;

				return o;
			}

			sampler2D _PaintCutoffTex;

			fixed4 _Color;
			float2 _Center;
			float _Wipe;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.screenPos * _ScreenParams.xy;
				float2 center = _Center;
				
				float unitDistance = distance(float2(0, 0), center);
				unitDistance = max(unitDistance, distance(float2(_ScreenParams.x, 0), center));
				unitDistance = max(unitDistance, distance(float2(0, _ScreenParams.y), center));
				unitDistance = max(unitDistance, distance(float2(_ScreenParams.x, _ScreenParams.y), center));
				float thisDistance = distance(uv, center);
				float t = thisDistance / unitDistance;

				float wiggle1 = tex2D(_PaintCutoffTex, uv * 0.0010 + float2(_Time.x * +0.2, _Time.x * -0.8)).r;
				float wiggle2 = tex2D(_PaintCutoffTex, uv * 0.0005 + float2(_Time.x * -0.4, _Time.x * +0.6)).r;
				float wiggleAmount = _Wipe * (1 - _Wipe) * 0.5;
				float w = (wiggle1 + wiggle2) / 2;
				w = (w - 0.5) * wiggleAmount;

				clip(t - _Wipe + w);
				return _Color;
			}

			ENDCG
		}
	}
}