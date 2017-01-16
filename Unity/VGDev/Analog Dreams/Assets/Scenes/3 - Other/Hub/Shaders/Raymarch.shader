Shader "Custom/Raymarch"
{
	Properties
	{
	}

	Subshader
	{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos       : POSITION;
				float4 screenPos : TEXCOORD0;
			};

			// Vertex shader
			// Just calculates position data

			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.screenPos = ComputeScreenPos(o.pos);
				return o;
			}
			
			// Functions that raymarch the local-space fog based on collider extents
			// and the scrolling fog textures passed in as samplers

			/*float mapFog(float3 p, float4x4 local, float3 extents)
			{
				float3 q = mul(local, float4(p, 1.0)).xyz;
				float3 d = abs(q) - abs(extents * 0.9);
				return min(max(d.x, max(d.y, d.z)), 0.0) + length(max(d, 0.0));
			}

			float traceFog(float3 origin, float3 look, float4x4 local, float3 extents)
			{
				float t = 0.0;
				for (int i = 0; i < 32; i++)
				{
					float3 p = origin + look * t;
					float d = mapFog(p, local, extents);
					t += d * 0.5;
				}
				return t;
			}*/

			// Functions that raymarch the world-space starfield which is clamped at clipping planes

			float mapStars(float3 p, float4x4 local, float3 extents)
			{
				float3 camPos = _WorldSpaceCameraPos.xzy * float3(-1, -1, 1);
				camPos = mul(local, float4(camPos, 1.0)).xyz;
				float3 localPos = mul(local, float4(p, 1.0)).xyz;
				localPos.y += 8.5;
				localPos.z -= _Time.x * 5;

				float stars = length(frac(localPos * 0.2) * 2.0 - 1.0) - 0.015;
				return stars;

				//float3 clip = (localPos * sign(camPos) - extents) * step(0, abs(camPos) - extents);
				//float clipping = max(clip.x, max(clip.y, clip.z));
				//return max(stars, clipping);
			}

			float traceStars(float3 origin, float3 look, float4x4 local, float3 extents)
			{
				float t = 0.0;
				for (int i = 0; i < 48; i++)
				{
					float3 p = origin + look * t;
					float d = mapStars(p, local, extents);
					t += d * 0.99;
				}
				return t;
			}

			// Test

			float4 clipping(float4x4 local, float3 extents)
			{
				float3 p = _WorldSpaceCameraPos.xzy * float3(-1, -1, 1);
				float3 q = mul(local, float4(p, 1.0)).xyz;
				float3 r = max(abs(q) - extents, 0) * sign(q);
				float3 s = r * 100 + 0.5;

				return float4(s, 1.0);
			}

			// Fragment shader
			
			sampler2D _GrabTexture;
			float4x4 _CameraMatrix;
			float4x4 _LocalMatrix;
			float3 _LocalExtents;
			float _AspectRatio;

			float4 frag(v2f i) : COLOR
			{
				// Get the screen coordinates for this pixel and then
				// project them into the useful space for raymarching

				float2 tex = i.screenPos.xy / i.screenPos.w;
				tex = (tex * 2.0 - 1.0) * float2(_AspectRatio, 1);

				// Create the origin vector

				float3 origin = _WorldSpaceCameraPos.xzy * float3(-1, -1, 1);

				// Create the look vector for this pixel
				// It's based on the texture coordinate

				float3 r = normalize(float3(tex, 1.3)).xzy * float3(-1, -1, 1);
				float3 look = mul(float4(r, 1.0), _CameraMatrix).xyz;

				// Raymarch!

				float t = traceStars(origin, look, _LocalMatrix, _LocalExtents.xzy);
				float fog = 1.0 / (1.0 + t * t * 0.02);
				return float4(0, fog, fog, 1.0);
			}

			ENDCG
		}
	}
}