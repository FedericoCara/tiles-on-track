﻿Shader "Mimic/NormalizedColor"
{
	Properties
	{
		[HideInInspector]_MainTex ("MainTex", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_UnaffectedThreshold("Unaffected Threshold", Range(0,1.01)) = 1
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma target 2.0
			#pragma multi_compile _ PIXELSNAP_ON
			//#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 uv  : TEXCOORD0;
			};
			
			fixed4 _Color;
			float _UnaffectedThreshold;

			sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _AlphaTex;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				//OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				color.a = tex2D (_AlphaTex, uv).r;
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.uv);
				fixed4 tint = IN.color;
				if(tint.r>=_UnaffectedThreshold && tint.g>=_UnaffectedThreshold && tint.b>=_UnaffectedThreshold){
					c.rgb *= c.a;
					return c;
				}else{
					float media = (c.r+c.g+c.b)/3;
					c.rgb = (c.r+(media-c.r)/3,c.g+(media-c.g)/3,c.b+(media-c.b)/3);
					c *= tint;
					c.rgb *= c.a;
					return c;
				}
			}
		ENDCG
		}
	}
}
