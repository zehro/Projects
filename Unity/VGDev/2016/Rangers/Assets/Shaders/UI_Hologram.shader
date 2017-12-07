Shader "UI/HoloShader"
 {
     Properties
     {
         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
         _Color ("Tint", Color) = (1,1,1,1)
         _Noise ("Noise", 2D) = "white" {}

         _StencilComp ("Stencil Comparison", Float) = 8
         _Stencil ("Stencil ID", Float) = 0
         _StencilOp ("Stencil Operation", Float) = 0
         _StencilWriteMask ("Stencil Write Mask", Float) = 255
         _StencilReadMask ("Stencil Read Mask", Float) = 255
 
         _ColorMask ("Color Mask", Float) = 15
     }
 
     SubShader
     {
          Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]
 
         Pass
         {
         CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #include "UnityCG.cginc"
             
             struct appdata_t
             {
                 float4 vertex   : POSITION;
                 float4 color    : COLOR;
                 float2 texcoord : TEXCOORD0;
             };
 
             struct v2f
             {
                 float4 vertex   : SV_POSITION;
                 fixed4 color    : COLOR;
                 half2 texcoord  : TEXCOORD0;
                 float3 wpos	 : TEXCOORD1;
             };

             float rand(float3 co)
			 {
			     return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
			 }
             
             fixed4 _Color;
             fixed4 _TextureSampleAdd;
             sampler2D _Noise;
             float randVal;
 
             v2f vert(appdata_t IN)
             {
                 v2f OUT;
                 OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
                 float3 worldPos = mul (_Object2World, IN.vertex).xyz;
                 OUT.wpos = worldPos;
                 OUT.texcoord = IN.texcoord;
 				#ifdef UNITY_HALF_TEXEL_OFFSET
                 OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
 				#endif

                 OUT.color = IN.color * _Color;
//                 randVal = rand(IN.vertex.xyz);
//                 half noi = tex2Dlod(_Noise,_Time.xyzw).x - 0.5;
//                 OUT.texcoord.xy += noi.x/200.;
//                 OUT.vertex.xy += tex2D(_Noise,half2(0.1,0.1)).x;
                 return OUT;
             }
 
             sampler2D _MainTex;


              float ramp(float y, float start, float end)
			{
				float inside = step(start,y) - step(end,y);
				float fact = (y-start)/(end-start)*inside;
				return (1.-fact) * inside;
				
			}

             float3 stripes(half2 uv)
			 {
				float3 noi = tex2D(_Noise,uv+rand(_Time.xyz));
				return ramp(((uv.y*5. + _Time*2.)%(1.)),0.3,0.6)*noi;
			 }

			 float noise(half2 p)
			 {
				float4 sample = tex2D(_Noise,p+rand(_Time.xyz));
				sample *= sample;
				return sample.x;
			 }
 
             fixed4 frag(v2f IN) : SV_Target
             {         
             	 if(randVal == 0) {    	
             	 	randVal = rand(IN.wpos);
             	 }
                 half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                 color.rgb += noise(IN.texcoord)*color.a/10;
                 color.rgb += stripes(IN.texcoord)*color.a/2;
                 clip (color.a - 0.01);
                 return color;
             }
         ENDCG
         }
     }
 }