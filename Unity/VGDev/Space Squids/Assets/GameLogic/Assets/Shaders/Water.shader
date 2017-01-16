// This shader is a derivative of the custom rim light shader
// which refracts what's on the screen behind it and also
// perturbs slightly to create the illusion of waves

Shader "Custom/Water"
{
	Properties
	{
		_Color ("Tint", Color) = (1,1,1,1)
		_Smoothness ("Gloss", Range(0,1)) = 0
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpIntensity ("Normal Intensity", Range(0.01,2)) = 1
		_RimColor ("Rim Color", Color) = (1,1,1,1)
		_RimPower ("Rim Power", Range(0.5,8)) = 3
		_RimStrength ("Rim Strength", Range(0,1)) = 1
		_WaveSize ("Wave Turbulence",Range(1,100)) = 20
		_WaveStrength ("Wave Amplitude",Range(0,2)) = 1
		_WaveSpeed ("Wave Speed",Range(0,1)) = 1
		_RefractStrength("Refraction Strength",Range(0,64)) = 32
	}
	
	Subshader
	{
		// Water is in the transparent queue, rendering after geometry
		// This is necessary so that it can refract things properly
		
		Tags
		{
			"Queue" = "Overlay"
			"RenderType" = "Transparent"
		}
		
		// Grab the screen and store it in the texture _GrabTexture (done automatically)
		// This is so the water can refract what's behind it, perturbing with the normal
		
		GrabPass
		{
		}
		
		// First pass (vertex/fragment shader)
		// The vertex shader makes the water wavy and gets the screen position of the vertex for refraction.
		// The fragment shader then draws the refraction of the screen texture and applies the water's tint color.
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct v2f
			{
				float4 pos       : POSITION;
				float2 uv        : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
				float4 vertColor : Color;
			};
			
			// Vertex shader
			
			float4 _BumpMap_ST;
			float _WaveSize;
			float _WaveStrength;
			float _WaveSpeed;
			
			v2f vert(appdata_full v)
			{
				v2f o;
				
				float4 s = v.vertex;
				float t = _Time.z*_WaveSpeed;
				float a = _WaveStrength/2*v.color.r;
				s.x += sin((s.x+s.y)*0.01*_WaveSize*1.125+t)*a;
				s.y += sin((s.y+s.z)*0.01*_WaveSize*1.000+t)*a;
				s.z += sin((s.z+s.x)*0.01*_WaveSize*1.250+t)*a;
				
				o.pos = mul(UNITY_MATRIX_MVP,s);
				o.uv = TRANSFORM_TEX(v.texcoord,_BumpMap);
				o.screenPos = ComputeGrabScreenPos(o.pos);
				o.vertColor = v.color;
				
				return o;
			}
			
			// Fragment shader
			
			float4 _Color;
			sampler2D _BumpMap;
			float _BumpIntensity;
			float _RefractStrength;
			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			
			float4 frag(v2f i) : COLOR
			{
				float2 NCoord1 = (i.uv+float2(0.4,-0.2)*_Time.x*_WaveSpeed);
				float2 NCoord2 = (i.uv+float2(-0.8,0.6)*_Time.x*_WaveSpeed)*2.5;
				float3 N1 = UnpackNormal(tex2D(_BumpMap,NCoord1))*0.5;
				float3 N2 = UnpackNormal(tex2D(_BumpMap,NCoord2))*0.5;
				float2 NFinal = normalize((N1+N2)/float3(1,1,_BumpIntensity)).xy*_RefractStrength*_GrabTexture_TexelSize.xy;
				
				float2 screenCoord = (i.screenPos.xy)/i.screenPos.w;
				return tex2D(_GrabTexture,screenCoord+NFinal*i.vertColor.r)*lerp(float4(1,1,1,1),_Color,_Color.a*i.vertColor.r);
			}
			
			ENDCG
		}
		
		// Second pass (surface shader)
		// Draw the physically based shading and rim lighting onto the water
		
		CGPROGRAM
		#pragma surface surf Standard alpha vertex:vert
		#pragma target 3.0
		#include "UnityCG.cginc"
		
		struct Input
		{
			float4 pos;
			float2 uv_BumpMap;
			float3 viewDir;
			float4 vertColor : Color;
		};
		
		float _WaveSize;
		float _WaveStrength;
		float _WaveSpeed;
		
		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			
			float4 s = v.vertex;
			float t = _Time.z*_WaveSpeed;
			float a = _WaveStrength/2*v.color.r;
			s.x += sin((s.x+s.y)*0.01*_WaveSize*1.125+t)*a;
			s.y += sin((s.y+s.z)*0.01*_WaveSize*1.000+t)*a;
			s.z += sin((s.z+s.x)*0.01*_WaveSize*1.250+t)*a;
			
			v.vertex = s;
		}
		
		fixed4 _Color;
		half _Smoothness;
		sampler2D _BumpMap;
		half _BumpIntensity;
		fixed4 _RimColor;
		half _RimPower;
		half _RimStrength;

		void surf(Input i, inout SurfaceOutputStandard o)
		{
			o.Smoothness = _Smoothness*i.vertColor.r;
			
			float2 NCoord1 = (i.uv_BumpMap+float2(0.4,-0.2)*_Time.x*_WaveSpeed);
			float2 NCoord2 = (i.uv_BumpMap+float2(-0.8,0.6)*_Time.x*_WaveSpeed)*2.5;
			float3 N1 = UnpackNormal(tex2D(_BumpMap,NCoord1))*0.5;
			float3 N2 = UnpackNormal(tex2D(_BumpMap,NCoord2))*0.5;
			o.Normal = normalize((N1+N2)/float3(1,1,_BumpIntensity*max(0.001,i.vertColor.r)));
			
			half rim = 1-saturate(dot(i.viewDir,o.Normal));
			o.Emission = max(float3(0.1,0.1,0.1),_RimColor.rgb*pow(rim*i.vertColor.r,_RimPower)*_RimStrength);
		}
		
		ENDCG
	}
	
	FallBack "Diffuse"
}