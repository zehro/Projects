Shader "Custom/UIBalloon"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_NoiseStrength("Noise Strength", Range(0,1)) = 0.25
		_Threshold("Threshold", Range(0,1)) = 0
		_CutoffDistance("Cutoff Distance", Float) = 1.5

		// (unused) required for masking

		_Stencil("Stencil ID", Float) = 0
		_StencilComp("Stencil Comparison", Float) = 8
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255
		_ColorMask("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
		}

		LOD 100

		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		ColorMask[_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			// Vertex shader

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 wPos : TEXCOORD1;
				float4 scrPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.wPos = mul(unity_ObjectToWorld, v.vertex);
				o.scrPos = ComputeScreenPos(v.vertex);
				return o;
			}

			// Fragment shader

			float4 _Color;
			sampler2D _NoiseTex;
			float4 _NoiseTex_TexelSize;
			float _NoiseStrength;
			float _Threshold;
			float _CutoffDistance;

			float4 _BirdPosition;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = (i.scrPos.xy / i.scrPos.w) * _NoiseTex_TexelSize.x * 2;
				float2 uv1 = uv * +1 + float2(_Time.x * +0.8, _Time.x * +0.3);
				float2 uv2 = uv * -1 + float2(_Time.x * -0.5, _Time.x * -0.7);
				float noise = (tex2D(_NoiseTex, uv1).r + tex2D(_NoiseTex, uv2).r) * 0.5;

				clip(tex2D(_MainTex, i.uv) - noise * _NoiseStrength - _Threshold);

				float dist = distance(_WorldSpaceCameraPos, i.wPos);
				clip(dist - _CutoffDistance - noise * 0.25);

				dist = distance(_BirdPosition, i.wPos);
				clip(dist - 1.25 - noise * 0.25);

				return _Color;
			}

			ENDCG
		}
	}
}