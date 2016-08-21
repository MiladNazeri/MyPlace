﻿#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "Unlit/Reflective" {
	Properties {
    	_EmissionColor ("Emission Color", Color) = (1,1,1,1)
    	_EmissionIntensity ("Emission Intensity", Range(0.0,1.0)) =0.05
    	_ReflectColor ("Reflection Color", Color) = (0.5,0.5,0.5,1.0)
		_ReflectCube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
	}
	SubShader {
		Pass {
		Tags { "RenderType"="Geometry"}
			
			CGPROGRAM
			#pragma noambient noforwardadd nofog
			#pragma vertex vert
			#pragma fragment frag
			
			/////Uniform
			//Emission
			uniform fixed4 _EmissionColor;
			uniform fixed _EmissionIntensity;
			//Reflect
			uniform fixed4 _ReflectColor;
			uniform samplerCUBE _ReflectCube;
			
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float3 I : TEXCOORD1;
			};
			
			vertexOutput vert(vertexInput v){
				vertexOutput o;
				//Set position
				o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
				
				//Calculate useful variables
				half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - mul(_Object2World,v.vertex).xyz);
				
				//Calculate reflective color
				float3 worldN = mul((float3x3)_Object2World, v.normal * 1.0);
				o.I = reflect( -viewDirection, worldN );
				
				//Calculate emission color
				half3 emissionColor=_EmissionColor*_EmissionIntensity;
				
				//Calculate final color
				o.col =float4(emissionColor,1.0);
				return o;
			}
			
			float4 frag(vertexOutput o) : COLOR
			{	
				fixed4 reflcol = texCUBE( _ReflectCube, o.I );
				return o.col + (reflcol * _ReflectColor);
			}
			ENDCG
		}
	}
}