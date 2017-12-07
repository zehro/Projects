// Per pixel bumped refraction.
// Uses a normal map to distort the image behind, and
// an additional texture to tint the color.

Shader "HeatDistortCustom" {
Properties {
	_BumpAmt  ("Distortion", range (0,128)) = 10
	_MainTex ("Tint Color (RGB)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
}


SubShader {
      Pass {	
         Tags { "LightMode" = "ForwardBase" } 
            // make sure that all uniforms are correctly set

         GLSLPROGRAM

         uniform vec4 _Color; // shader property specified by users

         // The following built-in uniforms (except _LightColor0) 
         // are also defined in "UnityCG.glslinc", 
         // i.e. one could #include "UnityCG.glslinc" 
         uniform mat4 _Object2World; // model matrix
         uniform mat4 _World2Object; // inverse model matrix
         uniform vec4 _WorldSpaceLightPos0; 
            // direction to or position of light source
         uniform vec4 _LightColor0; 
            // color of light source (from "Lighting.cginc")
         
         varying vec4 color; 
            // the diffuse lighting computed in the vertex shader
         
         #ifdef VERTEX
         
         void main()
         {				
            mat4 modelMatrix = _Object2World;
            mat4 modelMatrixInverse = _World2Object; // unity_Scale.w
               // is unnecessary because we normalize vectors
            
            vec3 normalDirection = normalize(
               vec3(vec4(gl_Normal, 0.0) * modelMatrixInverse));
            vec3 lightDirection = normalize(
               vec3(_WorldSpaceLightPos0));

            vec3 diffuseReflection = vec3(_LightColor0) * vec3(_Color)
               * max(0.0, dot(normalDirection, lightDirection));
            
            color = vec4(diffuseReflection, 1.0);
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         
         #endif

         #ifdef FRAGMENT
         
         void main()
         {
            gl_FragColor = color;
         }
         
         #endif

         ENDGLSL
      }
   } 

}
