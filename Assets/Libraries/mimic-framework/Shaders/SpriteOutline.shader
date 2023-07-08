Shader "Mimic/Sprite Outline" {

  Properties {
    [PerRendererData]_MainTex("Sprite Texture", 2D) = "white" {}

    _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
    _OutlineWidth("Outline Width", Range(1, 2)) = 1.1

  }

  SubShader {
    Tags {
      "Queue" = "Transparent+110"
      "RenderType" = "Transparent"
      "DisableBatching" = "True"
    }

    Pass {
      Name "Fill"
      Cull Off
      ZTest [_ZTest]
      ZWrite Off
      Blend SrcAlpha OneMinusSrcAlpha
      ColorMask RGBA

      Stencil {
        Ref 1
        Comp NotEqual
      }

      CGPROGRAM
      #include "UnityCG.cginc"

      #pragma vertex vert
      #pragma fragment frag

      struct appdata {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        float3 smoothNormal : TEXCOORD3;
        fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
      };

      struct v2f {
        float4 position : SV_POSITION;
        fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
      };

      uniform fixed4 _OutlineColor;
      uniform float _OutlineWidth;
			sampler2D _MainTex;
			float4 _MainTex_ST;

      v2f vert(appdata input) {
        v2f output;

        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        output.position = UnityObjectToClipPos(input.vertex);
        output.color = _OutlineColor;
				output.uv = TRANSFORM_TEX(input.uv, _MainTex);

        return output;
      }

      fixed4 frag(v2f input) : SV_Target {
        fixed4 textColor = tex2D(_MainTex, input.uv);
        fixed4 scaledTextColor = tex2D(_MainTex, (input.uv - 0.5) / _OutlineWidth + 0.5);
        fixed4 col = input.color;
        col.a = (1 - textColor.a) * scaledTextColor.a;
        return col;
      }
      
      ENDCG
    }
  }
}
