Shader "Mimic/AngleFilledSprite"
{
    Properties
    {
        [HideInInspector]_MainTex ("MainTex", 2D) = "white" {}
        _Angle ("Angle", Range(-90,90)) = 0
        [ToggleUI]  _InvertCheck ("InvertCheck", Float) = 0
        [ToggleUI]  _InvertFill ("InvertFill", Float) = 0
        _Fill ("Fill", Range(0,1)) = 1
    }
    SubShader
    {
        Tags {                         
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Angle;
            float _InvertCheck;
            float _Fill;
            float _InvertFill;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            void CheckAgainstAngle_float(float1 angle, bool invertCheck, bool InvertFill, float2 uv, float fill, out bool Out) {
                if(InvertFill){
                    fill = 1-fill;
                }
	            if (angle >= 0) {
		            if (invertCheck) {
			            Out = ((uv.x - (1 - fill))* tan(angle) + fill) <= uv.y;
		            }
		            else {
			            Out = ((uv.x - (1 - fill))* tan(angle) + fill) >= uv.y;
		            }
	            }
	            else {
		            if (invertCheck) {
			            Out = ((uv.x - fill)* tan(angle) + fill) <= uv.y;
		            }
		            else {
			            Out = ((uv.x - fill)* tan(angle) + fill) >= uv.y;
		            }
	            }
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float2 uv = i.uv;
                float angleInRadians = radians(_Angle);
                bool shouldBeOn;
                CheckAgainstAngle_float(angleInRadians, _InvertCheck, _InvertFill, uv, _Fill, shouldBeOn);
                
                fixed4 col = shouldBeOn? tex2D(_MainTex, i.uv) : fixed4(1,1,1,0);

                return col;
            }
            
            

            ENDCG
        }
    }
}
