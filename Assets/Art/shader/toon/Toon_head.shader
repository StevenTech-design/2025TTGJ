Shader "Universal Render Pipeline/Custom/ToonHeadShader"
{
    Properties
    {   
        [Header(Texture)]
        _BaseMap ("基础色", 2D) = "white" {}
        _ao ("ao",2D) = "gray"{}

        [Header(Diffuse)]
        _Toon_Theshold("中间阴影范围",range(0,1)) = 0.5
        _Toon_Hardness("阴影硬边过渡强度",float) = 80.0
        _colorA("阴影颜色",Color)= (0.5,0.5,0.5,1)
        _colorB("中间阴影颜色",Color)= (0.5,0.5,0.5,1)

        [Header(Specular)]
        _specInt("高光强度",float) = 0.0
        Pow_spec("高光范围",Range(0.01,100)) = 1.0
        
        [Header(Fresnel)]
        [Enum(Off, 0, On, 1)]fresnelOFF ("边缘光 OFF or ON",Int) = 0 
        _FresnelPow ("边缘光范围", Range(0, 10))= 0
        _FresnelColor("边缘光颜色",color) = (1,1,1,0)

        [Header(Outline)]
        _OutlineWidth ("外描边宽度", Range(0, 10)) = 0.5
        [HDR]_OutLineColor ("外描边颜色强度", color) = (0.5,0,0,1)

        [Header(PostProcess)]
        _colorSaturation("饱和度" ,range(0,10)) = 2.5

        [Header(PaintingSystem)]
        _PaintStrength ("染色强度", Range(0, 1)) = 1.0
        _PaintColorID ("颜色ID", Range(0, 1)) = 0.0

        [Header(Transparency)]
        _Alpha("透明度", float) = 1.0
        _Cutoff("透明裁剪阈值", Range(0, 1)) = 0.5  // 新增

    }
    
    SubShader
    {
        Tags 
        { 
        "RenderType" = "TransparentCutout"  // 改为裁剪透明
        "RenderPipeline" = "UniversalPipeline"
        "Queue" = "AlphaTest"  // 改为Alpha测试队列
        }


        LOD 200
        AlphaToMask On
        // 主Pass
        Pass
        {

            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }


            Cull Off

            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _RECEIVE_SHADOWS_OFF

            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers gles
            #pragma target 2.0


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float3 normalOS : NORMAL;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1; // 第二套UV：x存储颜色ID
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD2;
                float3 normalWS : TEXCOORD3;
                float4 shadowCoord : TEXCOORD4;
                float4 vertexColor : COLOR;   // alpha存储染色强度
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_ao);
            SAMPLER(sampler_ao);
            
            CBUFFER_START(UnityPerMaterial)
                float _Toon_Theshold;
                float _Toon_Hardness;
                float4 _colorA;
                float4 _colorB;
                float Pow_spec;
                float _specInt;
                float _colorSaturation;
                half4 _FresnelColor;
                float _FresnelPow;
                float fresnelOFF;

                float _PaintStrength ; // 直接从材质属性获取
                float _PaintColorID        ;// 直接从材质属性获取

                float _Alpha;
                float _Cutoff;



            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv0 = input.uv0;
                output.uv1 = input.uv1;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                
                // URP中的阴影坐标计算
                output.shadowCoord = TransformWorldToShadowCoord(output.positionWS);
                output.vertexColor = input.color;
                return output;
            }

             float3 GetPaintColor(float colorID)
            {
                float3 tomatoColors[7] = {
                    float3(1.0, 0.2, 0.2),   // 0: 红
                    float3(1.0, 0.5, 0.2),   // 1: 橙
                    float3(1.0, 0.9, 0.2),   // 2: 黄  
                    float3(0.3, 0.8, 0.3),   // 3: 绿
                    float3(0.2, 0.7, 0.8),   // 4: 青
                    float3(0.3, 0.4, 1.0),   // 5: 蓝
                    float3(0.7, 0.3, 1.0)    // 6: 紫
                };
    
                // 直接整数索引
                int index = clamp(int(colorID * 7.0), 0, 6);
                return tomatoColors[index];
            }


            half4 frag (Varyings input) : SV_Target
            {
                // 获取主光源信息
                Light mainLight = GetMainLight(input.shadowCoord);
   
                
                // 采样纹理
                half3 BaseCol = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv0).xyz;


                half ao = SAMPLE_TEXTURE2D(_ao, sampler_ao, input.uv0).r;

                //染色
                float3 paintColor = GetPaintColor(_PaintColorID);
                float3 finalBaseCol = lerp(paintColor, BaseCol, _PaintStrength);
                
                // 向量计算
                float3 normalDir = normalize(input.normalWS);
                float3 lightDir = normalize(mainLight.direction);
                float3 view_dir = normalize(GetWorldSpaceNormalizeViewDir(input.positionWS));
                

                half shadowAtten = mainLight.shadowAttenuation * mainLight.distanceAttenuation;
                shadowAtten = smoothstep(0.1,0.7,shadowAtten);
                half selfMask = saturate(dot(normalize(input.normalWS), normalize(_MainLightPosition.xyz)));//lambert
                // 当角度接近90°时认为是自遮挡，让阴影影响更小
                shadowAtten = lerp(shadowAtten, 0.5, pow(1 - selfMask , 3.0));
                
                

                // 漫反射光照
                half ndotl = dot(normalDir, lightDir) ;
                half toon_diffuse = lerp(_colorA, 1.0, saturate(ndotl * _Toon_Hardness)) ;
                half toon_middiffuse = lerp(_colorB, 1.0, saturate((ndotl - _Toon_Theshold) * _Toon_Hardness));
                half final_toon = toon_diffuse * toon_middiffuse ;
                final_toon *= shadowAtten;
                // 镜面反射光照
                float3 half_dir = normalize(lightDir + view_dir);
                half ndoth = dot(normalDir, half_dir);
                half3 spec_color = step(0.8, pow(max(0.0, ndoth), Pow_spec));
                half3 spec = saturate(ndotl * spec_color * _specInt);

                // 边缘光
                float vdotn = dot(view_dir, normalDir);
                float3 fresnel = pow(max(0.0, 1.0 - vdotn), _FresnelPow) * _FresnelColor ;

                // 最终颜色合成
                half3 final = (finalBaseCol * final_toon * mainLight.color + spec) + fresnel * fresnelOFF ;
                final = sqrt(max(exp2(log2(max(final, 0.0)) * _colorSaturation), 0.0))  ;

                clip(_Alpha - _Cutoff);
                return half4(final,_Alpha);
            }
            ENDHLSL
        }

        // 轮廓线Pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="SRPDefaultUnlit" }
            
            Cull Front
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
                half4 outlineColor : TEXCOORD1;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            
            CBUFFER_START(UnityPerMaterial)
                float _OutlineWidth;
                half4 _OutLineColor;
                float _Alpha;
            CBUFFER_END



            Varyings vert (Attributes input)
            {
                Varyings output;
                
                // URP中的轮廓线计算 - 在观察空间进行法线扩展
                float3 positionOS = input.positionOS.xyz;
                float3 normalOS = input.normalOS;
                
                // 将法线转换到观察空间并进行扩展
                float3 positionVS = TransformWorldToView(TransformObjectToWorld(positionOS));
                float3 normalVS = TransformWorldToViewDir(TransformObjectToWorldNormal(normalOS));
                //float outlineStrength = UNITY_ACCESS_INSTANCED_PROP(Props, _OutLineColor);
                positionVS += normalVS * _OutlineWidth * 0.01;
                output.positionHCS = TransformWViewToHClip(positionVS);
                output.uv = input.uv0;
                output.outlineColor = _OutLineColor; // HDR 高亮
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // 轮廓线颜色计算
                float3 BaseCol = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).xyz;
                half maxComponent = max(max(BaseCol.r, BaseCol.g), BaseCol.b) - 0.004;
                half3 saturatedColor = step(maxComponent.rrr, BaseCol) * BaseCol;
                saturatedColor = lerp(BaseCol.rgb, saturatedColor, 0.6);
                
                half3 outlineColor = 0.8 * saturatedColor * BaseCol * input.outlineColor.rgb;
                return half4(outlineColor,_Alpha);
            }
            ENDHLSL
        }

        
    }
    FallBack "Universal Render Pipeline/Lit"
}