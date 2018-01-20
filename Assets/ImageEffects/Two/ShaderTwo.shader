Shader "ErikW/Shaders/EffectTwoShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Tags 
		{
			"Queue" = "Transparent"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float4 _Color;

			float4 boxBlur (sampler2D tex, float2 uv, float4 size)
			{
				float4 c = 	tex2D (tex, uv + float2 (-size.x, size.y)) + tex2D (tex, uv + float2 (0, size.y)) + tex2D (tex, uv + float2 (size.x, size.y)) +
							tex2D (tex, uv + float2 (-size.x, 0)) + tex2D (tex, uv + float2 (0, 0)) + tex2D (tex, uv + float2 (size.x, 0)) +
							tex2D (tex, uv + float2 (-size.x, -size.y)) + tex2D (tex, uv + float2 (0, -size.y)) + tex2D (tex, uv + float2 (size.x, -size.y));

				return c/9;
			} 

			float4 sqrBlur (sampler2D tex, float2 uv, float4 size)
			{
				float4 c = 	pow(tex2D (tex, uv + float2 (-size.x, size.y)), 2) + pow(tex2D (tex, uv + float2 (0, size.y)), 2) + pow(tex2D (tex, uv + float2 (size.x, size.y)), 2) +
							pow(tex2D (tex, uv + float2 (-size.x, 0)), 2) + pow(tex2D (tex, uv + float2 (0, 0)), 2) + pow(tex2D (tex, uv + float2 (size.x, 0)), 2) +
							pow(tex2D (tex, uv + float2 (-size.x, -size.y)), 2) + pow(tex2D (tex, uv + float2 (0, -size.y)), 2) + pow(tex2D (tex, uv + float2 (size.x, -size.y)), 2);

				return sqrt(c/9);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				
				//	Set the colors accordingly matching those of the src texture

					float4 col = tex2D(_MainTex, i.uv);
					float l = 0.3 * col.x + 0.59 * col.y + 0.11 * col.z;

					col = float4 (l, l, l, col.a) * _Color;
				
				//	float4 col = boxBlur (_MainTex, i.uv, _MainTex_TexelSize);
				//	float4 col = boxBlur (_MainTex, i.uv, _MainTex_TexelSize);
				return col;
			}
			ENDCG
		}
	}
}
