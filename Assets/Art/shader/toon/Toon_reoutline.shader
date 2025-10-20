Shader "Universal Render Pipeline/Custom/Toon_reoutline"
{
    Properties
    {   
        [Header(Texture)]
        _BaseMap ("基础色", 2D) = "white" {}
        _ao ("ao",2D) = "gray"{}

        [Header(Diffuse)]
        _Toon_Theshold("中间阴影范围",range(0,1)) = 0.5
        _Toon_Hardness("阴影硬边过渡强度",float) = 20.0
        _colorA("阴影颜色",Color)= (0.5,0.5,0.5,1)
        _colorB("中间阴影颜色",Color)= (0.5,0.5,0.5,1)

        [Header(Specular)]
        _specInt("高光强度",float) = 1.0
        Pow_spec("高光范围",Range(0.01,100)) = 1.0
        
        [Header(Fresnel)]
        [Enum(Off, 0, On, 1)]fresnelOFF ("边缘光 OFF or ON",Int) = 1 
        _FresnelPow ("边缘光范围", Range(0, 10))= 1
        _FresnelColor("边缘光颜色",color) = (1,1,1,0)

        [Header(Outline)]
        _OutlineWidth ("外描边宽度", Range(0, 10)) = 0.24
        _OutLineColor ("外描边颜色强度", color) = (0.5,0.5,0.5,1)

        [Header(PostProcess)]
        _colorSaturation("饱和度" ,range(0,10)) = 2.5
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Geometry"
        }
        LOD 200

        // 主Pass
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float2 uv0 : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
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
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv0 = input.uv0;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                
                // URP中的阴影坐标计算
                output.shadowCoord = TransformWorldToShadowCoord(output.positionWS);
                
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // 获取主光源信息
                Light mainLight = GetMainLight(input.shadowCoord);
                
                // 采样纹理
                half3 BaseCol = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv0).xyz;
                half ao = SAMPLE_TEXTURE2D(_ao, sampler_ao, input.uv0).r;
                
                // 向量计算
                float3 normalDir = normalize(input.normalWS);
                float3 lightDir = normalize(mainLight.direction);
                float3 view_dir = normalize(GetWorldSpaceNormalizeViewDir(input.positionWS));
                
                // 阴影衰减 (URP中的阴影处理)
                //float atten = mainLight.shadowAttenuation * mainLight.distanceAttenuation;

                // 漫反射光照
                half ndotl = dot(normalDir, lightDir) ;
                half toon_diffuse = lerp(_colorA, 1.0, saturate(ndotl * _Toon_Hardness));
                half toon_middiffuse = lerp(_colorB, 1.0, saturate((ndotl - _Toon_Theshold) * _Toon_Hardness));
                half final_toon = toon_diffuse * toon_middiffuse;

                // 镜面反射光照
                float3 half_dir = normalize(lightDir + view_dir);
                half ndoth = dot(normalDir, half_dir);
                half3 spec_color = step(0.8, pow(max(0.0, ndoth), Pow_spec));
                half3 spec = saturate(ndotl * spec_color * _specInt);

                // 边缘光
                float vdotn = dot(view_dir, normalDir);
                float3 fresnel = pow(max(0.0, 1.0 - vdotn), _FresnelPow) * _FresnelColor;

                // 最终颜色合成
                half3 final = BaseCol * final_toon * mainLight.color + spec + fresnel * fresnelOFF;
                final = sqrt(max(exp2(log2(max(final, 0.0)) * _colorSaturation), 0.0));
                
                return half4(final, 1.0);
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
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;  // 使用切线数据（存储了平均法线）
            };

            struct Varyings
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
           
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            half _OutlineWidth;
            half4 _OutLineColor;

            // 智能轮廓线颜色生成 - 根据基础色调整轮廓线颜色
            half3 GenerateSmartOutlineColor(half3 basecol)
            {
                // 计算亮度
                float luminance = dot(basecol, half3(0.299, 0.587, 0.114));
        
                // 智能颜色规则：
                if (luminance > 0.7) {
                    // 高亮度物体：使用暗色轮廓（如白色衣服用灰色轮廓）
                    return basecol * 0.3;
                }
                else if (luminance < 0.3) {
                    // 低亮度物体：使用亮色轮廓（如黑色头发用深灰色轮廓）
                    return basecol * 1.8;
                }
                else {
                    // 中等亮度：使用变暗的基础色，保持色相
                    return basecol * 0.5;
                }
            }

            Varyings vert (Attributes v)
            {
                Varyings o;
        
                // 使用切线数据中的平均法线（来自PlugTangentTool处理）
                // 切线数据的xyz存储了平均法线，w分量通常为0
                float3 smoothedNormal = v.tangent.xyz;
        
                // URP版本：在裁剪空间进行法线扩展
                float4 pos = TransformObjectToHClip(v.vertex.xyz);
        
                // 将平均法线转换到观察空间
                float3 worldNormal = TransformObjectToWorldNormal(smoothedNormal);
                float3 viewNormal = TransformWorldToViewDir(worldNormal);
        
                // 将法线变换到NDC空间
                float3 ndcNormal = normalize(mul((float3x3)UNITY_MATRIX_P, viewNormal));
        
                // 调整屏幕宽高比
                float aspect = _ScreenParams.x / _ScreenParams.y;
                ndcNormal.x *= aspect;
        
                // 应用轮廓线宽度 - 使用平均法线获得更平滑的轮廓
                pos.xy += 0.01 * _OutlineWidth * ndcNormal.xy;
                o.pos = pos;
                o.uv = v.uv;
        
                return o;
            }

            half4 frag(Varyings i) : SV_TARGET
            {
                // 采样基础贴图颜色
                half3 basecol = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv).rgb;
        
                // 生成智能轮廓线颜色
                half3 smartOutlineColor = GenerateSmartOutlineColor(basecol);
        
                // 混合原始轮廓线颜色和智能颜色（70%智能颜色 + 30%原始颜色）
                half3 finalOutlineColor = lerp(_OutLineColor.rgb, smartOutlineColor, 0.1);
        
                return half4(finalOutlineColor, 1.0);
            }
            ENDHLSL
        }


    }
  
    
    FallBack "Universal Render Pipeline/Lit"
}