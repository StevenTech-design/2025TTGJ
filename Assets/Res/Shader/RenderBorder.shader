Shader "Unlit/GridCellBorder"
{
    Properties {
        _Color("Border Color", Color) = (0,1,1,1)
        _Border("Border Width", Range(0,0.5)) = 0.05
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            float _Border;
            fixed4 _Color;

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // 距离边缘的距离
                float uDist = min(i.uv.x, 1 - i.uv.x);
                float vDist = min(i.uv.y, 1 - i.uv.y);

                // 判断是否在边框区域
                float edgeMask = (uDist < _Border || vDist < _Border) ? 1.0 : 0.0;

                if (edgeMask < 0.5)
                    discard; // 丢弃非边界像素，保持中间透明

                return _Color;
            }
            ENDHLSL
        }
    }
}