Shader "Custom/BubbleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BubbleColor ("Bubble Base Color", Color) = (1,1,1,0.2)
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
        _DistortionStrength ("Distortion Strength", Range(0, 1)) = 0.1
        _FresnelScale ("Fresnel Scale", Range(0, 1)) = 0.5
        _ChromaticAberration ("Chromatic Aberration", Range(0, 0.1)) = 0.03
        _BubbleSpeed ("Bubble Animation Speed", Range(0, 2)) = 0.5
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        // 启用透明混合
        Blend SrcAlpha OneMinusSrcAlpha
        // 禁用深度写入，但保持深度测试
        ZWrite Off
        Cull Back
        
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
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float4 worldPos : TEXCOORD3;
                float4 screenPos : TEXCOORD4;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BubbleColor;
            float4 _RimColor;
            float _RimPower;
            float _DistortionStrength;
            float _FresnelScale;
            float _ChromaticAberration;
            float _BubbleSpeed;
            
            // 彩虹颜色函数
            float3 rainbow(float t) 
            {
                float r = sin(t * 6.28318) * 0.5 + 0.5;
                float g = sin(t * 6.28318 + 2.0944) * 0.5 + 0.5;
                float b = sin(t * 6.28318 + 4.18879) * 0.5 + 0.5;
                return float3(r, g, b);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 基础纹理
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // 计算法线与视角的夹角，用于菲涅尔效果
                float3 worldNormal = normalize(i.worldNormal);
                float3 viewDir = normalize(i.viewDir);
                float fresnel = pow(1.0 - saturate(dot(worldNormal, viewDir)), _RimPower);
                
                // 添加时间动画效果
                float time = _Time.y * _BubbleSpeed;
                
                // 彩虹色效果
                float3 rainbowColor = rainbow(fresnel + time * 0.1);
                
                // 扭曲效果
                float2 offset = worldNormal.xy * _DistortionStrength;
                offset.x += sin(i.worldPos.y * 10 + time) * 0.01;
                offset.y += cos(i.worldPos.x * 10 + time) * 0.01;
                
                // 色差效果
                float3 chromatic;
                chromatic.r = rainbow(fresnel + time * 0.1 + _ChromaticAberration).r;
                chromatic.g = rainbowColor.g;
                chromatic.b = rainbow(fresnel + time * 0.1 - _ChromaticAberration).b;
                
                // 混合所有效果
                float3 finalColor = lerp(_BubbleColor.rgb, chromatic, fresnel * _FresnelScale);
                
                // 边缘高光
                finalColor = lerp(finalColor, _RimColor.rgb, pow(fresnel, 2) * _RimColor.a);
                
                // 最终透明度
                float alpha = _BubbleColor.a + fresnel * 0.5;
                
                return float4(finalColor, alpha);
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}