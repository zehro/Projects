Shader "ProcBlock/ProcBlockInvisible"
{
	Properties
	{
		_RefractionTex("Refraction", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{ 
			"Queue" = "Transparent+10"
		}

		GrabPass
		{
			"_BackgroundTex"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 grabPos : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				half3 worldNormal : TEXCOORD2;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.grabPos = ComputeGrabScreenPos(o.pos);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}

			sampler2D _RefractionTex;
			sampler2D _BackgroundTex;

			half4 frag(v2f i) : SV_Target
			{
				float3 w = i.worldPos.xyz;
				float2 uv = 0;
				uv.x = (w.x * 0.4 + w.y * 0.5 + w.z * 0.6) * 0.5 + _Time.x;
				uv.y = (w.x * 0.6 + w.y * 0.5 + w.z * 0.4) * 0.5 + _Time.x;
				float distortion = tex2D(_RefractionTex, uv).r;

				float4 screenUV = i.grabPos + distortion * 0.05 - float4(i.worldNormal.xyz * 0.1, 0);
				return tex2Dproj(_BackgroundTex, screenUV) + i.worldNormal.y * 0.01;
			}

			ENDCG
		}
	}
}