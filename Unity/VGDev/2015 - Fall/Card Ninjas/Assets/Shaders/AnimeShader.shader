Shader "Custom/AnimeShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
        _Amount ("Extrusion Amount", Range(-1,1)) = 0.5
	}	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		
        ZTest LEqual
		CGPROGRAM
	    #pragma surface surf Lambert vertex:vert
	    struct Input {
	        float2 uv_MainTex;
	    };
	    float _Amount;
	    void vert (inout appdata_full v) {
	        v.vertex.xyz += v.normal * _Amount;
	    }
	    sampler2D _MainTex;
		fixed4 _ColorS;
		
	    void surf (Input IN, inout SurfaceOutput o) {
	        o.Albedo = fixed3(0,0,0);
	    }
	    ENDCG
        Offset 0, -10000
	    CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
          	float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
        Offset 0, 10000
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
          	float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}