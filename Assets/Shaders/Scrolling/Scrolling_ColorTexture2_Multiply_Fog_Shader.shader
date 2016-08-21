// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Scrolling/Color Texture 2 Multiply Fog" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Color2 ("Main Color", Color) = (1,1,1,1)
	_MainTex2 ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _Color2;
			sampler2D _MainTex2;
			float4 _MainTex2_ST;

			
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.texcoord = v.texcoord;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 coords = _MainTex_ST.xy*i.texcoord.xy + _MainTex_ST.zw*_Time.y;
				fixed4 mainTex = _Color*tex2D(_MainTex, coords);
				
				coords = _MainTex2_ST.xy*i.texcoord.xy + _MainTex2_ST.zw*_Time.y;
				fixed4 mainTex2 = _Color2*tex2D(_MainTex2, coords);
				
				fixed4 col = lerp(mainTex,mainTex2,.5*mainTex2.a);//Less of two if the alpha is low
				
				UNITY_APPLY_FOG(i.fogCoord, col);
				
				return col;
			}
		ENDCG
	}
}

}