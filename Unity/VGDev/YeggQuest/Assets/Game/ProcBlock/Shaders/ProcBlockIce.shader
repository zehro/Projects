Shader "ProcBlock/ProcBlockIce"
{
	Properties
	{
		_Color("Tint", Color) = (1, 1, 1, 1)
		_Smoothness("Smoothness", Range(0, 1)) = 0
		_NormalTex("Normal Tex", 2D) = "bump" {}
		_NormalStrength("Normal Strength", Range(0.01, 2)) = 1
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower("Rim Power", Range(0.5, 8)) = 3
		_RimStrength("Rim Strength", Range(0, 1)) = 1
		_RefractStrength("Refraction Strength", Range(0, 256)) = 32
	}

	Subshader
	{
		Tags
		{
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
		}

		/*GrabPass
		{
		}*/

		/*Pass
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
			};

			// Vertex shader

			float4 _BumpMap_ST;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.grabPos = ComputeGrabScreenPos(o.pos);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			// Fragment shader

			float4 _Color;
			sampler2D _NormalTex;
			float _NormalStrength;
			float _RefractStrength;
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;

			float4 frag(v2f i) : COLOR
			{
				float4 w = i.worldPos;
				float2 uv = 0;
				uv.x = w.x * 0.4 + w.y * 0.5 + w.z * 0.6;
				uv.y = w.x * 0.6 + w.y * 0.5 + w.z * 0.4;

				float3 n = UnpackNormal(tex2D(_NormalTex, uv));
				float2 refract = normalize(n).xy * _RefractStrength * _GrabTexture_TexelSize.xy;

				float4 screenUV = i.grabPos;
				screenUV.xy += refract;
				return lerp(tex2Dproj(_GrabTexture, screenUV), _Color, _Color.a);
			}

			ENDCG
		}*/
		
		// PASS 2

		CGPROGRAM
		#pragma surface surf Standard vertex:vert
		#pragma target 3.0
		#include "UnityCG.cginc"

		struct Input
		{
			float3 viewDir;
			float3 wPos;
		};

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);

			o.wPos = mul(unity_ObjectToWorld, v.vertex);
		}

		fixed4 _Color;
		half _Smoothness;
		sampler2D _NormalTex;
		float _NormalStrength;
		fixed4 _RimColor;
		half _RimPower;
		half _RimStrength;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half3 w = IN.wPos * 0.75;
			float2 uv = 0;
			uv.x = w.x * 0.4 + w.y * 0.5 + w.z * 0.6;
			uv.y = w.x * 0.6 + w.y * 0.5 + w.z * 0.4;

			o.Albedo = _Color;
			o.Smoothness = _Smoothness;

			float3 norm = UnpackNormal(tex2D(_NormalTex, uv));
			o.Normal = normalize(norm / float3(1, 1, _NormalStrength));

			half rim = 1 - saturate(dot(IN.viewDir, o.Normal));
			o.Emission = max(float3(0.1, 0.1, 0.1), _RimColor.rgb * pow(rim, _RimPower) * _RimStrength);
		}

		ENDCG
	}

	FallBack "Diffuse"
}