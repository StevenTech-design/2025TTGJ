// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Twater"
{
	Properties
	{
		_DeepColor("DeepColor", Color) = (0,0.579416,1,0)
		_WaveASpeedXYSteepnesswavelength("WaveA(SpeedXY,Steepness,wavelength)", Vector) = (1,1,2,50)
		_RippleTex("_RippleTex", 2D) = "white" {}
		_WaveB("WaveB", Vector) = (1,1,2,50)
		_DeepRange("DeepRange", Float) = 10
		_ShallowColor("ShallowColor", Color) = (0.1729558,1,0.8981904,0)
		_WaveC("WaveC", Vector) = (1,1,2,50)
		_FresnelCol("FresnelCol", Color) = (1,1,1,0)
		_FresnelPower("FresnelPower", Float) = 5
		_NormalScale("NormalScale", Float) = 30
		_NormalSpeed("NormalSpeed", Vector) = (10,10,0,0)
		_NormalMap("NormalMap", 2D) = "white" {}
		_ReflectionTex("ReflectionTex", 2D) = "white" {}
		_UniderWaterDistort("UniderWaterDistort", Float) = 1
		_ReflectionDisTort("ReflectionDisTort", Float) = 1
		_ReflectInt("ReflectInt", Float) = 1
		_ReflectPower("ReflectPower", Float) = 5
		_CousticsTex("CousticsTex", 2D) = "white" {}
		_CousticsScale("CousticsScale", Float) = 1
		_CousticsInt("CousticsInt", Float) = 1
		_CousticsRange("CousticsRange", Float) = 1
		_CousticsSpeed("CousticsSpeed", Vector) = (-8,0,0,0)
		_ShoreCol("ShoreCol", Color) = (1,1,1,0)
		_ShoreRange("ShoreRange", Float) = 1
		_ShoreEdgeWidth("ShoreEdgeWidth", Range( 0 , 1)) = 1
		_ShoreEdgeInt("ShoreEdgeInt", Float) = 0
		_FoamBlend("FoamBlend", Range( 0 , 1)) = 0
		_FoamSpeed("FoamSpeed", Float) = -1
		_FoamFrequency("FoamFrequency", Float) = 20
		_FoamRange("FoamRange", Float) = 1
		_FoamNoise("FoamNoise", Vector) = (10,10,0,0)
		_FoamDissolve("FoamDissolve", Float) = 1
		_FoamWidth("FoamWidth", Float) = 1.1
		_FoamColor("FoamColor", Color) = (1,1,1,1)
		_CoustExp("CoustExp", Range( 0 , 100)) = 1
		_boguanInt("boguanInt", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.5
		#define ASE_VERSION 19801
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Unlit alpha:fade keepalpha vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		uniform float4 _WaveASpeedXYSteepnesswavelength;
		uniform float4 _WaveB;
		uniform float4 _WaveC;
		uniform float4 _DeepColor;
		uniform float4 _ShallowColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _DeepRange;
		uniform float4 _FresnelCol;
		uniform float _FresnelPower;
		uniform sampler2D _ReflectionTex;
		uniform sampler2D _NormalMap;
		uniform float _NormalScale;
		uniform float2 _NormalSpeed;
		uniform sampler2D _RippleTex;
		uniform float _ReflectionDisTort;
		uniform float _ReflectInt;
		uniform float _ReflectPower;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _UniderWaterDistort;
		uniform sampler2D _CousticsTex;
		uniform float _CousticsScale;
		uniform float2 _CousticsSpeed;
		uniform float _CousticsInt;
		uniform float _CousticsRange;
		uniform float _CoustExp;
		uniform float _boguanInt;
		uniform float4 _ShoreCol;
		uniform float _ShoreRange;
		uniform float _FoamBlend;
		uniform float _FoamRange;
		uniform float _FoamWidth;
		uniform float _FoamFrequency;
		uniform float _FoamSpeed;
		uniform float2 _FoamNoise;
		uniform float _FoamDissolve;
		uniform float4 _FoamColor;
		uniform float _ShoreEdgeWidth;
		uniform float _ShoreEdgeInt;


		float3 GerstnerWave185( float3 position, inout float3 tangent, inout float3 binormal, float4 wave )
		{
			float steepness = wave.z * 0.01;
			float wavelength = wave.w;
			float k = 2 * UNITY_PI / wavelength;
			float c = sqrt(9.8 / k);
			float2 d = normalize(wave.xy);
			float f = k * (dot(d, position.xz) - c * _Time.y);
			float a = steepness / k;
						
			tangent += float3(
			-d.x * d.x * (steepness * sin(f)),
			d.x * (steepness * cos(f)),
			-d.x * d.y * (steepness * sin(f))
			);
			binormal += float3(
			-d.x * d.y * (steepness * sin(f)),
			d.y * (steepness * cos(f)),
			-d.y * d.y * (steepness * sin(f))
			);
			return float3(
			d.x * (a * cos(f)),
			a * sin(f),
			d.y * (a * cos(f))
			);
		}


		float3 GerstnerWave187( float3 position, inout float3 tangent, inout float3 binormal, float4 wave )
		{
			float steepness = wave.z * 0.01;
			float wavelength = wave.w;
			float k = 2 * UNITY_PI / wavelength;
			float c = sqrt(9.8 / k);
			float2 d = normalize(wave.xy);
			float f = k * (dot(d, position.xz) - c * _Time.y);
			float a = steepness / k;
						
			tangent += float3(
			-d.x * d.x * (steepness * sin(f)),
			d.x * (steepness * cos(f)),
			-d.x * d.y * (steepness * sin(f))
			);
			binormal += float3(
			-d.x * d.y * (steepness * sin(f)),
			d.y * (steepness * cos(f)),
			-d.y * d.y * (steepness * sin(f))
			);
			return float3(
			d.x * (a * cos(f)),
			a * sin(f),
			d.y * (a * cos(f))
			);
		}


		float3 GerstnerWave203( float3 position, inout float3 tangent, inout float3 binormal, float4 wave )
		{
			float steepness = wave.z * 0.01;
			float wavelength = wave.w;
			float k = 2 * UNITY_PI / wavelength;
			float c = sqrt(9.8 / k);
			float2 d = normalize(wave.xy);
			float f = k * (dot(d, position.xz) - c * _Time.y);
			float a = steepness / k;
						
			tangent += float3(
			-d.x * d.x * (steepness * sin(f)),
			d.x * (steepness * cos(f)),
			-d.x * d.y * (steepness * sin(f))
			);
			binormal += float3(
			-d.x * d.y * (steepness * sin(f)),
			d.y * (steepness * cos(f)),
			-d.y * d.y * (steepness * sin(f))
			);
			return float3(
			d.x * (a * cos(f)),
			a * sin(f),
			d.y * (a * cos(f))
			);
		}


		float2 UnStereo( float2 UV )
		{
			#if UNITY_SINGLE_PASS_STEREO
			float4 scaleOffset = unity_StereoScaleOffset[ unity_StereoEyeIndex ];
			UV.xy = (UV.xy - scaleOffset.zw) / scaleOffset.xy;
			#endif
			return UV;
		}


		float3 InvertDepthDir72_g1( float3 In )
		{
			float3 result = In;
			#if !defined(ASE_SRP_VERSION) || ASE_SRP_VERSION <= 70301
			result *= float3(1,1,-1);
			#endif
			return result;
		}


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		//https://www.shadertoy.com/view/XdXGW8
		float2 GradientNoiseDir( float2 x )
		{
			const float2 k = float2( 0.3183099, 0.3678794 );
			x = x * k + k.yx;
			return -1.0 + 2.0 * frac( 16.0 * k * frac( x.x * x.y * ( x.x + x.y ) ) );
		}
		
		float GradientNoise( float2 UV, float Scale )
		{
			float2 p = UV * Scale;
			float2 i = floor( p );
			float2 f = frac( p );
			float2 u = f * f * ( 3.0 - 2.0 * f );
			return lerp( lerp( dot( GradientNoiseDir( i + float2( 0.0, 0.0 ) ), f - float2( 0.0, 0.0 ) ),
					dot( GradientNoiseDir( i + float2( 1.0, 0.0 ) ), f - float2( 1.0, 0.0 ) ), u.x ),
					lerp( dot( GradientNoiseDir( i + float2( 0.0, 1.0 ) ), f - float2( 0.0, 1.0 ) ),
					dot( GradientNoiseDir( i + float2( 1.0, 1.0 ) ), f - float2( 1.0, 1.0 ) ), u.x ), u.y );
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_positionWS = mul( unity_ObjectToWorld, v.vertex );
			float3 position185 = ase_positionWS;
			float3 tangent185 = float3( 1,0,0 );
			float3 binormal185 = float3( 0,0,1 );
			float4 wave185 = _WaveASpeedXYSteepnesswavelength;
			float3 localGerstnerWave185 = GerstnerWave185( position185 , tangent185 , binormal185 , wave185 );
			float3 position187 = ase_positionWS;
			float3 tangent187 = tangent185;
			float3 binormal187 = binormal185;
			float4 wave187 = _WaveB;
			float3 localGerstnerWave187 = GerstnerWave187( position187 , tangent187 , binormal187 , wave187 );
			float3 position203 = ase_positionWS;
			float3 tangent203 = tangent187;
			float3 binormal203 = binormal187;
			float4 wave203 = _WaveC;
			float3 localGerstnerWave203 = GerstnerWave203( position203 , tangent203 , binormal203 , wave203 );
			float3 temp_output_189_0 = ( ase_positionWS + localGerstnerWave185 + localGerstnerWave187 + localGerstnerWave203 );
			float3 worldToObj198 = mul( unity_WorldToObject, float4( temp_output_189_0, 1 ) ).xyz;
			float3 WaveVertexPos201 = worldToObj198;
			v.vertex.xyz = ( float3( 0,0,0 ) + WaveVertexPos201 );
			v.vertex.w = 1;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_positionWS = i.worldPos;
			float4 ase_positionSS = float4( i.screenPos.xyz , i.screenPos.w + 1e-7 );
			float4 ase_positionSSNorm = ase_positionSS / ase_positionSS.w;
			ase_positionSSNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_positionSSNorm.z : ase_positionSSNorm.z * 0.5 + 0.5;
			float2 UV22_g3 = ase_positionSSNorm.xy;
			float2 localUnStereo22_g3 = UnStereo( UV22_g3 );
			float2 break64_g1 = localUnStereo22_g3;
			float depth01_69_g1 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_positionSSNorm.xy );
			#ifdef UNITY_REVERSED_Z
				float staticSwitch38_g1 = ( 1.0 - depth01_69_g1 );
			#else
				float staticSwitch38_g1 = depth01_69_g1;
			#endif
			float3 appendResult39_g1 = (float3(break64_g1.x , break64_g1.y , staticSwitch38_g1));
			float4 appendResult42_g1 = (float4((appendResult39_g1*2.0 + -1.0) , 1.0));
			float4 temp_output_43_0_g1 = mul( unity_CameraInvProjection, appendResult42_g1 );
			float3 temp_output_46_0_g1 = ( (temp_output_43_0_g1).xyz / (temp_output_43_0_g1).w );
			float3 In72_g1 = temp_output_46_0_g1;
			float3 localInvertDepthDir72_g1 = InvertDepthDir72_g1( In72_g1 );
			float4 appendResult49_g1 = (float4(localInvertDepthDir72_g1 , 1.0));
			float3 PositionFormDepth4 = (mul( unity_CameraToWorld, appendResult49_g1 )).xyz;
			float clampResult7 = clamp( ( ase_positionWS.y - (PositionFormDepth4).y ) , 0.0 , 1.0 );
			float WaterDeepUV17 = clampResult7;
			float clampResult15 = clamp( exp( ( -WaterDeepUV17 / _DeepRange ) ) , 0.0 , 1.0 );
			float4 lerpResult10 = lerp( _DeepColor , _ShallowColor , clampResult15);
			float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_positionWS );
			float3 ase_viewDirWS = normalize( ase_viewVectorWS );
			float3 ase_normalWS = i.worldNormal;
			float fresnelNdotV20 = dot( ase_normalWS, ase_viewDirWS );
			float fresnelNode20 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV20, _FresnelPower ) );
			float4 lerpResult18 = lerp( lerpResult10 , _FresnelCol , fresnelNode20);
			float4 WaterCol22 = lerpResult18;
			float2 temp_output_30_0 = ( ( (ase_positionWS).xz * -0.1 ) / _NormalScale );
			float2 temp_output_35_0 = ( _NormalSpeed * _Time.y * 0.01 );
			float2 temp_output_2_0_g7 = i.uv_texcoord;
			float temp_output_30_0_g7 = 0.1;
			float4 appendResult24_g7 = (float4(0.01 , 0.0 , ( ( tex2D( _RippleTex, ( temp_output_2_0_g7 + float2( 0.01,0 ) ) ).r - tex2D( _RippleTex, ( temp_output_2_0_g7 + float2( -0.01,0 ) ) ).r ) * temp_output_30_0_g7 ) , 0.0));
			float4 appendResult26_g7 = (float4(0.0 , 0.01 , ( temp_output_30_0_g7 * ( tex2D( _RippleTex, ( temp_output_2_0_g7 + float2( 0,0.01 ) ) ).r - tex2D( _RippleTex, ( temp_output_2_0_g7 + float2( 0,-0.01 ) ) ).r ) ) , 0.0));
			float3 normalizeResult28_g7 = normalize( cross( appendResult24_g7.xyz , appendResult26_g7.xyz ) );
			float3 temp_output_279_0 = BlendNormals( BlendNormals( UnpackNormal( tex2D( _NormalMap, ( temp_output_30_0 + temp_output_35_0 ) ) ) , UnpackNormal( tex2D( _NormalMap, ( ( temp_output_30_0 * 2.0 ) + ( temp_output_35_0 * -0.5 ) ) ) ) ) , normalizeResult28_g7 );
			float3 SurfaceNormaL39 = temp_output_279_0;
			float fresnelNdotV76 = dot( ase_normalWS, ase_viewDirWS );
			float fresnelNode76 = ( 0.0 + _ReflectInt * pow( 1.0 - fresnelNdotV76, _ReflectPower ) );
			float clampResult79 = clamp( fresnelNode76 , 0.0 , 1.0 );
			float4 ReflectionCol58 = ( tex2D( _ReflectionTex, ( (ase_positionSSNorm).xy + ( (SurfaceNormaL39).xz * ( _ReflectionDisTort * 0.01 ) ) ) ) * clampResult79 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_positionSS );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor62 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + float4( ( SurfaceNormaL39 * _UniderWaterDistort * 0.01 ) , 0.0 ) ).xy);
			float4 UnderCol107 = screenColor62;
			float WaterOpacity332 = ( 1.0 - (WaterCol22).a );
			float4 lerpResult73 = lerp( ( float4( 0,0,0,0 ) + WaterCol22 + ReflectionCol58 ) , UnderCol107 , WaterOpacity332);
			float2 temp_output_82_0 = ( (PositionFormDepth4).xz / _CousticsScale );
			float2 temp_output_85_0 = ( _Time.y * _CousticsSpeed * 0.01 );
			float clampResult99 = clamp( exp( ( -WaterDeepUV17 / _CousticsRange ) ) , 0.0 , 1.0 );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_lightDirWS = 0;
			#else //aseld
			float3 ase_lightDirWS = normalize( UnityWorldSpaceLightDir( ase_positionWS ) );
			#endif //aseld
			float dotResult299 = dot( ase_lightDirWS , ase_viewDirWS );
			float4 temp_output_295_0 = ( ( ( min( tex2D( _CousticsTex, ( temp_output_82_0 + temp_output_85_0 ) ) , tex2D( _CousticsTex, ( -temp_output_82_0 + temp_output_85_0 ) ) ) * _CousticsInt ) * clampResult99 ) * ( pow( saturate( dotResult299 ) , _CoustExp ) * _boguanInt ) );
			float4 CousticsCol92 = temp_output_295_0;
			float3 ShoreCol120 = (( ( CousticsCol92 + UnderCol107 ) * _ShoreCol )).rgb;
			float clampResult113 = clamp( exp( ( -WaterDeepUV17 / _ShoreRange ) ) , 0.0 , 1.0 );
			float WaterShore114 = clampResult113;
			float4 lerpResult121 = lerp( lerpResult73 , float4( ShoreCol120 , 0.0 ) , WaterShore114);
			float clampResult139 = clamp( ( WaterDeepUV17 / _FoamRange ) , 0.0 , 1.0 );
			float smoothstepResult154 = smoothstep( _FoamBlend , 1.0 , ( clampResult139 + 0.1 ));
			float temp_output_142_0 = ( 1.0 - clampResult139 );
			float gradientNoise160 = GradientNoise(( i.uv_texcoord * _FoamNoise ),1.0);
			gradientNoise160 = gradientNoise160*0.5 + 0.5;
			float4 FoamCol173 = ( ( ( 1.0 - smoothstepResult154 ) * step( ( temp_output_142_0 - _FoamWidth ) , ( ( temp_output_142_0 + ( sin( ( ( temp_output_142_0 * _FoamFrequency ) + ( _Time.y * _FoamSpeed ) ) ) + gradientNoise160 ) ) - _FoamDissolve ) ) ) * _FoamColor );
			float4 lerpResult174 = lerp( lerpResult121 , ( lerpResult121 + float4( (FoamCol173).rgb , 0.0 ) ) , (FoamCol173).a);
			float smoothstepResult126 = smoothstep( ( 1.0 - _ShoreEdgeWidth ) , 1.0 , WaterShore114);
			float ShoreEdge131 = ( smoothstepResult126 * _ShoreEdgeInt );
			o.Emission = max( ( lerpResult174 + ShoreEdge131 ) , float4( 0,0,0,0 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.CommentaryNode;207;-480,-848;Inherit;False;2185.023;451.5732;WaterDeepUV;8;17;7;5;6;1;4;3;2;WaterDeepUV;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;2;-320,-544;Inherit;False;Reconstruct World Position From Depth;-1;;1;e7094bcbcc80eb140b2a3dbe6a861de8;0;0;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;209;-528,864;Inherit;False;2569.806;742.9132;SurfaceNormal;19;46;40;38;45;32;42;41;44;43;35;30;61;31;34;37;36;60;28;27;SurfaceNormal;1,1,1,1;0;0
Node;AmplifyShaderEditor.SwizzleNode;3;96,-544;Inherit;False;FLOAT3;0;1;2;3;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;27;-288,928;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;4;320,-544;Inherit;False;PositionFormDepth;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;28;-48,928;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-80,1056;Inherit;False;Constant;_Float3;Float 3;9;0;Create;True;0;0;0;False;0;False;-0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;6;592,-544;Inherit;False;FLOAT;1;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1;512,-720;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;37;-16,1424;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;36;16,1328;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;144,1088;Inherit;False;Property;_NormalScale;NormalScale;10;0;Create;True;0;0;0;False;0;False;30;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;144,960;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;34;48,1184;Inherit;False;Property;_NormalSpeed;NormalSpeed;11;0;Create;True;0;0;0;False;0;False;10,10;-8,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;212;-464,3664;Inherit;False;2461.818;1126.777;CousticsCol;31;91;97;90;99;104;98;101;95;89;96;94;103;84;93;102;85;82;100;88;87;83;81;80;241;300;299;303;301;297;302;305;CousticsCol;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;5;864,-640;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;352,1472;Inherit;False;Constant;_Float2;Float 2;7;0;Create;True;0;0;0;False;0;False;-0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;288,1232;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;30;400,976;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;43;304,1136;Inherit;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;-400,3984;Inherit;False;4;PositionFormDepth;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;7;1120,-640;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;576,1408;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;576,1248;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;215;3536,1968;Inherit;False;3242.768;1815.522;FoamCol;31;173;178;179;165;155;168;154;171;166;172;156;157;169;167;164;147;160;162;145;163;161;151;144;142;150;143;153;139;138;141;137;FoamCol;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-192,4112;Inherit;False;Property;_CousticsScale;CousticsScale;19;0;Create;True;0;0;0;False;0;False;1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;81;-144,3984;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;1408,-624;Inherit;False;WaterDeepUV;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;32;656,1024;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;45;768,1360;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;87;0,4192;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;88;0,4400;Inherit;False;Constant;_Float6;Float 6;12;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;100;-16,4272;Inherit;False;Property;_CousticsSpeed;CousticsSpeed;22;0;Create;True;0;0;0;False;0;False;-8,0;-8,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleDivideOpNode;82;96,3984;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;137;3616,2384;Inherit;False;17;WaterDeepUV;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;141;3616,2544;Inherit;False;Property;_FoamRange;FoamRange;30;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;38;912,1056;Inherit;True;Property;_NormalMap;NormalMap;12;0;Create;True;0;0;0;False;0;False;-1;2aab2b9fb283e6646ad7f1df2f799dc1;229ead3cefce9dc4a97ab7f982338757;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;40;912,1328;Inherit;True;Property;_NormalMap1;NormalMap;12;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Instance;38;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TexturePropertyNode;275;608,1536;Inherit;True;Property;_RippleTex;_RippleTex;2;0;Create;False;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexCoordVertexDataNode;273;608,1744;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;208;-480,-256;Inherit;False;3411.18;949.9296;WaterCol WaterOpacity;17;22;18;19;20;10;21;15;9;8;16;13;14;12;11;330;331;332;WaterCol   WaterOpacity;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;224,4240;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NegateNode;102;288,4096;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;0,4496;Inherit;False;17;WaterDeepUV;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;138;3856,2480;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;46;1312,1280;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;274;1152,1632;Inherit;False;Height To Normal;-1;;7;2a591e36b51461f489e4aed5ffc45a74;0;3;30;FLOAT;0.1;False;1;SAMPLER2D;0,0,0,0;False;2;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;297;992,4192;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;11;-432,320;Inherit;False;17;WaterDeepUV;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;84;384,3984;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;103;480,4160;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;96;224,4608;Inherit;False;Property;_CousticsRange;CousticsRange;21;0;Create;True;0;0;0;False;0;False;1;0.58;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;94;288,4480;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;300;1056,4352;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ClampOpNode;139;4016,2464;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;279;2320,1488;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;101;640,4112;Inherit;True;Property;_TextureSample0;Texture Sample 0;18;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;89;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;2736,1408;Inherit;False;SurfaceNormaL;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;211;-480,2784;Inherit;False;2256.034;682.2825;UnderWater;10;70;105;107;62;64;66;63;67;68;65;UnderWater;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-224,432;Inherit;False;Property;_DeepRange;DeepRange;4;0;Create;True;0;0;0;False;0;False;10;0.34;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;12;-176,304;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;95;496,4480;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;299;1344,4224;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;143;4448,3104;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;150;4448,3232;Inherit;False;Property;_FoamSpeed;FoamSpeed;28;0;Create;True;0;0;0;False;0;False;-1;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;142;4368,2640;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;153;4400,2992;Inherit;False;Property;_FoamFrequency;FoamFrequency;29;0;Create;True;0;0;0;False;0;False;20;23.17;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;89;624,3888;Inherit;True;Property;_CousticsTex;CousticsTex;18;0;Create;True;0;0;0;False;0;False;-1;None;72417ee6f632e4e4aabc8c81143957bf;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.CommentaryNode;210;-480,1952;Inherit;False;2408.223;771.2708;ReflectionCol;17;58;75;79;47;76;57;77;78;49;54;48;52;51;56;55;50;292;ReflectionCol;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;303;1344,4384;Inherit;False;Property;_CoustExp;CoustExp;39;0;Create;True;0;0;0;False;0;False;1;0;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;65;-272,2864;Inherit;False;39;SurfaceNormaL;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-224,3120;Inherit;False;Property;_UniderWaterDistort;UniderWaterDistort;14;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-176,3248;Inherit;False;Constant;_Float5;Float 5;9;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;13;16,304;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;976,3872;Inherit;False;Property;_CousticsInt;CousticsInt;20;0;Create;True;0;0;0;False;0;False;1;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;104;1040,4016;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ExpOpNode;98;720,4496;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;301;1504,4176;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;144;4704,3136;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;161;4448,3344;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;163;4496,3488;Inherit;False;Property;_FoamNoise;FoamNoise;31;0;Create;True;0;0;0;False;0;False;10,10;40,40;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;4704,2880;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;80,2480;Inherit;False;Constant;_Float4;Float 4;8;0;Create;True;0;0;0;False;0;False;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;302;1712,4160;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;32,2368;Inherit;False;Property;_ReflectionDisTort;ReflectionDisTort;15;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;63;32,2832;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;128,3040;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;50;0,2272;Inherit;False;39;SurfaceNormaL;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ExpOpNode;16;192,304;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;1200,3952;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;99;928,4496;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;305;1712,4352;Inherit;False;Property;_boguanInt;boguanInt;40;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;4752,3376;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;145;5040,2992;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;304;1920,4160;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;64;352,2880;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;48;144,2016;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwizzleNode;51;256,2272;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;272,2400;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;9;304,64;Inherit;False;Property;_ShallowColor;ShallowColor;5;0;Create;True;0;0;0;False;0;False;0.1729558,1,0.8981904,0;0,0.780901,1,0.3529412;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;8;304,-160;Inherit;False;Property;_DeepColor;DeepColor;0;0;Create;True;0;0;0;False;0;False;0,0.579416,1,0;0,0.5882354,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;21;192,576;Inherit;False;Property;_FresnelPower;FresnelPower;9;0;Create;True;0;0;0;False;0;False;5;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;15;320,304;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;1456,3984;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;160;4944,3360;Inherit;False;Gradient;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;147;5168,2992;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;213;-448,5040;Inherit;False;2309.833;770.1641;ShoreEdge;20;131;129;130;126;120;127;114;119;128;113;116;112;118;115;110;111;109;108;311;312;ShoreEdge;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;512,2352;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;295;2080,4032;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;62;512,2864;Inherit;False;Global;_GrabScreen0;Grab Screen 0;9;0;Create;True;0;0;0;False;0;False;Object;-1;False;False;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwizzleNode;49;464,2064;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;78;688,2512;Inherit;False;Property;_ReflectInt;ReflectInt;16;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;688,2608;Inherit;False;Property;_ReflectPower;ReflectPower;17;0;Create;True;0;0;0;False;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;10;608,128;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;19;512,288;Inherit;False;Property;_FresnelCol;FresnelCol;8;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0.4529402,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.FresnelNode;20;512,512;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;164;5376,2976;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;76;960,2496;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;107;784,2864;Inherit;True;UnderCol;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;704,2176;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;18;832,128;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;92;2384,4048;Inherit;False;CousticsCol;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;108;-304,5232;Inherit;False;17;WaterDeepUV;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;157;4304,2320;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;4288,2496;Inherit;False;Property;_FoamBlend;FoamBlend;27;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;169;4400,2880;Inherit;False;Property;_FoamWidth;FoamWidth;33;0;Create;True;0;0;0;False;0;False;1.1;0.81;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;167;5520,2896;Inherit;False;Property;_FoamDissolve;FoamDissolve;32;0;Create;True;0;0;0;False;0;False;1;2.21;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;172;5536,2640;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;79;1216,2496;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;22;1120,144;Inherit;False;WaterCol;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NegateNode;109;-48,5232;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-112,5360;Inherit;False;Property;_ShoreRange;ShoreRange;24;0;Create;True;0;0;0;False;0;False;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;115;128,5584;Inherit;False;107;UnderCol;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;311;112,5488;Inherit;False;92;CousticsCol;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;154;4624,2352;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;171;4864,2688;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;166;5776,2656;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;47;880,2144;Inherit;True;Property;_ReflectionTex;ReflectionTex;13;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.CommentaryNode;182;3568,960;Inherit;False;2542.907;761.6924;Wave Vertex Animation ;17;203;201;200;199;198;196;194;192;191;190;189;188;187;186;185;184;183;Wave Vertex Animation ;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;1248,2224;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;312;320,5520;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SwizzleNode;330;1392,144;Inherit;False;FLOAT;3;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;110;192,5264;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;118;112,5680;Inherit;False;Property;_ShoreCol;ShoreCol;23;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.OneMinusNode;155;4832,2336;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;168;6064,2864;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;184;3680,1040;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector4Node;183;3648,1344;Inherit;False;Property;_WaveASpeedXYSteepnesswavelength;WaveA(SpeedXY,Steepness,wavelength);1;0;Create;True;0;0;0;False;0;False;1,1,2,50;0,-1,1.6,50;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;214;3728,-368;Inherit;False;2660.586;969.5335;FinalCol;20;136;204;132;133;174;175;180;177;121;176;125;124;73;74;71;72;59;23;252;280;FinalCol;0.1603772,1,0.4367865,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;432,5616;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;58;1472,2224;Inherit;False;ReflectionCol;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;331;1568,128;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ExpOpNode;112;368,5264;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;179;5888,2288;Inherit;False;Property;_FoamColor;FoamColor;34;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,0.3647059;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;165;6000,2528;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;186;4064,1424;Inherit;False;Property;_WaveB;WaveB;3;0;Create;True;0;0;0;False;0;False;1,1,2,50;-0.5,-0.5,1.6,50;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;185;4048,1248;Inherit;False;float steepness = wave.z * 0.01@$float wavelength = wave.w@$float k = 2 * UNITY_PI / wavelength@$float c = sqrt(9.8 / k)@$float2 d = normalize(wave.xy)@$float f = k * (dot(d, position.xz) - c * _Time.y)@$float a = steepness / k@$			$$tangent += float3($-d.x * d.x * (steepness * sin(f)),$d.x * (steepness * cos(f)),$-d.x * d.y * (steepness * sin(f))$)@$$binormal += float3($-d.x * d.y * (steepness * sin(f)),$d.y * (steepness * cos(f)),$-d.y * d.y * (steepness * sin(f))$)@$$return float3($d.x * (a * cos(f)),$a * sin(f),$d.y * (a * cos(f))$)@;3;Create;4;True;position;FLOAT3;0,0,0;In;;Inherit;False;True;tangent;FLOAT3;1,0,0;InOut;;Inherit;False;True;binormal;FLOAT3;0,0,1;InOut;;Inherit;False;True;wave;FLOAT4;0,0,0,0;In;;Inherit;False;GerstnerWave;True;False;0;;False;4;0;FLOAT3;0,0,0;False;1;FLOAT3;1,0,0;False;2;FLOAT3;0,0,1;False;3;FLOAT4;0,0,0,0;False;3;FLOAT3;0;FLOAT3;2;FLOAT3;3
Node;AmplifyShaderEditor.SwizzleNode;119;640,5616;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;332;1760,192;Inherit;False;WaterOpacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;113;512,5232;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;128;320,5376;Inherit;False;Property;_ShoreEdgeWidth;ShoreEdgeWidth;25;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;3760,-160;Inherit;False;22;WaterCol;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;59;3744,-64;Inherit;False;58;ReflectionCol;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;6272,2512;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;188;4480,1440;Inherit;False;Property;_WaveC;WaveC;6;0;Create;True;0;0;0;False;0;False;1,1,2,50;1,0.5,1,50;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;187;4384,1248;Inherit;False;float steepness = wave.z * 0.01@$float wavelength = wave.w@$float k = 2 * UNITY_PI / wavelength@$float c = sqrt(9.8 / k)@$float2 d = normalize(wave.xy)@$float f = k * (dot(d, position.xz) - c * _Time.y)@$float a = steepness / k@$			$$tangent += float3($-d.x * d.x * (steepness * sin(f)),$d.x * (steepness * cos(f)),$-d.x * d.y * (steepness * sin(f))$)@$$binormal += float3($-d.x * d.y * (steepness * sin(f)),$d.y * (steepness * cos(f)),$-d.y * d.y * (steepness * sin(f))$)@$$return float3($d.x * (a * cos(f)),$a * sin(f),$d.y * (a * cos(f))$)@;3;Create;4;True;position;FLOAT3;0,0,0;In;;Inherit;False;True;tangent;FLOAT3;1,0,0;InOut;;Inherit;False;True;binormal;FLOAT3;0,0,1;InOut;;Inherit;False;True;wave;FLOAT4;0,0,0,0;In;;Inherit;False;GerstnerWave;True;False;0;;False;4;0;FLOAT3;0,0,0;False;1;FLOAT3;1,0,0;False;2;FLOAT3;0,0,1;False;3;FLOAT4;0,0,0,0;False;3;FLOAT3;0;FLOAT3;2;FLOAT3;3
Node;AmplifyShaderEditor.SimpleAddOpNode;74;4016,-128;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;71;3744,32;Inherit;False;107;UnderCol;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;127;688,5376;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;120;864,5616;Inherit;False;ShoreCol;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;72;3760,128;Inherit;False;332;WaterOpacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;173;6464,2496;Inherit;False;FoamCol;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;114;720,5264;Inherit;False;WaterShore;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;203;4720,1216;Inherit;False;float steepness = wave.z * 0.01@$float wavelength = wave.w@$float k = 2 * UNITY_PI / wavelength@$float c = sqrt(9.8 / k)@$float2 d = normalize(wave.xy)@$float f = k * (dot(d, position.xz) - c * _Time.y)@$float a = steepness / k@$			$$tangent += float3($-d.x * d.x * (steepness * sin(f)),$d.x * (steepness * cos(f)),$-d.x * d.y * (steepness * sin(f))$)@$$binormal += float3($-d.x * d.y * (steepness * sin(f)),$d.y * (steepness * cos(f)),$-d.y * d.y * (steepness * sin(f))$)@$$return float3($d.x * (a * cos(f)),$a * sin(f),$d.y * (a * cos(f))$)@;3;Create;4;True;position;FLOAT3;0,0,0;In;;Inherit;False;True;tangent;FLOAT3;1,0,0;InOut;;Inherit;False;True;binormal;FLOAT3;0,0,1;InOut;;Inherit;False;True;wave;FLOAT4;0,0,0,0;In;;Inherit;False;GerstnerWave;True;False;0;;False;4;0;FLOAT3;0,0,0;False;1;FLOAT3;1,0,0;False;2;FLOAT3;0,0,1;False;3;FLOAT4;0,0,0,0;False;3;FLOAT3;0;FLOAT3;2;FLOAT3;3
Node;AmplifyShaderEditor.LerpOp;73;4192,-16;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;126;1008,5264;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;130;992,5408;Inherit;False;Property;_ShoreEdgeInt;ShoreEdgeInt;26;0;Create;True;0;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;124;3792,336;Inherit;False;114;WaterShore;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;125;3808,224;Inherit;False;120;ShoreCol;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;176;4320,304;Inherit;False;173;FoamCol;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;189;4976,1040;Inherit;False;4;4;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;1280,5280;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;121;4480,64;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SwizzleNode;177;4544,224;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;131;1472,5296;Inherit;False;ShoreEdge;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;175;4784,-112;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SwizzleNode;180;4560,336;Inherit;False;FLOAT;3;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;198;5216,1040;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;174;4992,16;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;133;5040,272;Inherit;False;131;ShoreEdge;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;201;5664,1040;Inherit;False;WaveVertexPos;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;132;5296,32;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;204;5984,144;Inherit;False;201;WaveVertexPos;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;190;4992,1296;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;191;5216,1248;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;192;5392,1264;Inherit;False;FLOAT;1;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;194;5568,1200;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;196;5440,1344;Inherit;False;Property;_WaveColor;WaveColor;7;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.5031445,0.5031445,0.5031445,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;199;5776,1264;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;200;5936,1264;Inherit;False;WaveColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldPosInputsNode;283;928,1792;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;286;1120,1920;Inherit;False;Property;_RainTilling;RainTilling;35;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;284;1168,1776;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;285;1376,1744;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FractNode;287;1568,1760;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;290;1536,1856;Inherit;False;Constant;_Float7;Float 7;38;0;Create;True;0;0;0;False;0;False;8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;292;1584,1968;Inherit;False;Property;_RainDropSpeed;RainDropSpeed;37;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;291;1536,1936;Inherit;False;Constant;_Float8;Float 7;38;0;Create;True;0;0;0;False;0;False;8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation;289;1760,1744;Inherit;False;0;0;7;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;-1;False;4;FLOAT2;0;FLOAT;1;FLOAT;2;INT;3
Node;AmplifyShaderEditor.RangedFloatNode;294;1840,1968;Inherit;False;Property;_RainInt;RainInt;38;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;288;2080,1680;Inherit;True;Property;_RainDropTex;RainDropTex;36;0;Create;True;0;0;0;False;0;False;-1;36470d9a8273acc4b9439246996bea8c;36470d9a8273acc4b9439246996bea8c;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.BlendNormalsNode;293;2656,1600;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;241;-304,3776;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;105;1008,3088;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;321;4672,4402;Inherit;False;Constant;_Float9;Float 9;43;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;322;5104,4338;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;324;4960,4466;Inherit;False;Property;_DisplacementAmount;_DisplacementAmount;42;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;323;5300.658,4358.045;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;325;5776,4338;Inherit;False;displacement;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;315;3952,4178;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;316;4192,4194;Inherit;True;Property;_displacement;displacement;41;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SwizzleNode;317;4624,4242;Inherit;False;FLOAT3;0;1;2;3;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;320;4880,4322;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;326;5248,4130;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;327;5552,4306;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;1200,3040;Inherit;False;UnderWaterCol;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;136;5616,0;Inherit;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;252;6240,128;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;280;6112,-224;Inherit;False;39;SurfaceNormaL;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;6528,-176;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;0;Unlit;Twater;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Absolute;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;2;0
WireConnection;4;0;3;0
WireConnection;28;0;27;0
WireConnection;6;0;4;0
WireConnection;61;0;28;0
WireConnection;61;1;60;0
WireConnection;5;0;1;2
WireConnection;5;1;6;0
WireConnection;35;0;34;0
WireConnection;35;1;36;0
WireConnection;35;2;37;0
WireConnection;30;0;61;0
WireConnection;30;1;31;0
WireConnection;7;0;5;0
WireConnection;42;0;35;0
WireConnection;42;1;44;0
WireConnection;41;0;30;0
WireConnection;41;1;43;0
WireConnection;81;0;80;0
WireConnection;17;0;7;0
WireConnection;32;0;30;0
WireConnection;32;1;35;0
WireConnection;45;0;41;0
WireConnection;45;1;42;0
WireConnection;82;0;81;0
WireConnection;82;1;83;0
WireConnection;38;1;32;0
WireConnection;40;1;45;0
WireConnection;85;0;87;0
WireConnection;85;1;100;0
WireConnection;85;2;88;0
WireConnection;102;0;82;0
WireConnection;138;0;137;0
WireConnection;138;1;141;0
WireConnection;46;0;38;0
WireConnection;46;1;40;0
WireConnection;274;1;275;0
WireConnection;274;2;273;0
WireConnection;84;0;82;0
WireConnection;84;1;85;0
WireConnection;103;0;102;0
WireConnection;103;1;85;0
WireConnection;94;0;93;0
WireConnection;139;0;138;0
WireConnection;279;0;46;0
WireConnection;279;1;274;0
WireConnection;101;1;103;0
WireConnection;39;0;279;0
WireConnection;12;0;11;0
WireConnection;95;0;94;0
WireConnection;95;1;96;0
WireConnection;299;0;297;0
WireConnection;299;1;300;0
WireConnection;142;0;139;0
WireConnection;89;1;84;0
WireConnection;13;0;12;0
WireConnection;13;1;14;0
WireConnection;104;0;89;0
WireConnection;104;1;101;0
WireConnection;98;0;95;0
WireConnection;301;0;299;0
WireConnection;144;0;143;0
WireConnection;144;1;150;0
WireConnection;151;0;142;0
WireConnection;151;1;153;0
WireConnection;302;0;301;0
WireConnection;302;1;303;0
WireConnection;66;0;65;0
WireConnection;66;1;67;0
WireConnection;66;2;68;0
WireConnection;16;0;13;0
WireConnection;90;0;104;0
WireConnection;90;1;91;0
WireConnection;99;0;98;0
WireConnection;162;0;161;0
WireConnection;162;1;163;0
WireConnection;145;0;151;0
WireConnection;145;1;144;0
WireConnection;304;0;302;0
WireConnection;304;1;305;0
WireConnection;64;0;63;0
WireConnection;64;1;66;0
WireConnection;51;0;50;0
WireConnection;52;0;55;0
WireConnection;52;1;56;0
WireConnection;15;0;16;0
WireConnection;97;0;90;0
WireConnection;97;1;99;0
WireConnection;160;0;162;0
WireConnection;147;0;145;0
WireConnection;54;0;51;0
WireConnection;54;1;52;0
WireConnection;295;0;97;0
WireConnection;295;1;304;0
WireConnection;62;0;64;0
WireConnection;49;0;48;0
WireConnection;10;0;8;0
WireConnection;10;1;9;0
WireConnection;10;2;15;0
WireConnection;20;3;21;0
WireConnection;164;0;147;0
WireConnection;164;1;160;0
WireConnection;76;2;78;0
WireConnection;76;3;77;0
WireConnection;107;0;62;0
WireConnection;57;0;49;0
WireConnection;57;1;54;0
WireConnection;18;0;10;0
WireConnection;18;1;19;0
WireConnection;18;2;20;0
WireConnection;92;0;295;0
WireConnection;157;0;139;0
WireConnection;172;0;142;0
WireConnection;172;1;164;0
WireConnection;79;0;76;0
WireConnection;22;0;18;0
WireConnection;109;0;108;0
WireConnection;154;0;157;0
WireConnection;154;1;156;0
WireConnection;171;0;142;0
WireConnection;171;1;169;0
WireConnection;166;0;172;0
WireConnection;166;1;167;0
WireConnection;47;1;57;0
WireConnection;75;0;47;0
WireConnection;75;1;79;0
WireConnection;312;0;311;0
WireConnection;312;1;115;0
WireConnection;330;0;22;0
WireConnection;110;0;109;0
WireConnection;110;1;111;0
WireConnection;155;0;154;0
WireConnection;168;0;171;0
WireConnection;168;1;166;0
WireConnection;116;0;312;0
WireConnection;116;1;118;0
WireConnection;58;0;75;0
WireConnection;331;0;330;0
WireConnection;112;0;110;0
WireConnection;165;0;155;0
WireConnection;165;1;168;0
WireConnection;185;0;184;0
WireConnection;185;3;183;0
WireConnection;119;0;116;0
WireConnection;332;0;331;0
WireConnection;113;0;112;0
WireConnection;178;0;165;0
WireConnection;178;1;179;0
WireConnection;187;0;184;0
WireConnection;187;1;185;2
WireConnection;187;2;185;3
WireConnection;187;3;186;0
WireConnection;74;1;23;0
WireConnection;74;2;59;0
WireConnection;127;0;128;0
WireConnection;120;0;119;0
WireConnection;173;0;178;0
WireConnection;114;0;113;0
WireConnection;203;0;184;0
WireConnection;203;1;187;2
WireConnection;203;2;187;3
WireConnection;203;3;188;0
WireConnection;73;0;74;0
WireConnection;73;1;71;0
WireConnection;73;2;72;0
WireConnection;126;0;114;0
WireConnection;126;1;127;0
WireConnection;189;0;184;0
WireConnection;189;1;185;0
WireConnection;189;2;187;0
WireConnection;189;3;203;0
WireConnection;129;0;126;0
WireConnection;129;1;130;0
WireConnection;121;0;73;0
WireConnection;121;1;125;0
WireConnection;121;2;124;0
WireConnection;177;0;176;0
WireConnection;131;0;129;0
WireConnection;175;0;121;0
WireConnection;175;1;177;0
WireConnection;180;0;176;0
WireConnection;198;0;189;0
WireConnection;174;0;121;0
WireConnection;174;1;175;0
WireConnection;174;2;180;0
WireConnection;201;0;198;0
WireConnection;132;0;174;0
WireConnection;132;1;133;0
WireConnection;191;0;189;0
WireConnection;191;1;190;0
WireConnection;192;0;191;0
WireConnection;194;0;192;0
WireConnection;199;0;194;0
WireConnection;199;1;196;0
WireConnection;200;0;199;0
WireConnection;284;0;283;0
WireConnection;285;0;284;0
WireConnection;285;1;286;0
WireConnection;287;0;285;0
WireConnection;289;0;287;0
WireConnection;289;1;290;0
WireConnection;289;2;291;0
WireConnection;289;3;292;0
WireConnection;288;1;289;0
WireConnection;288;5;294;0
WireConnection;293;0;279;0
WireConnection;293;1;288;0
WireConnection;105;0;107;0
WireConnection;105;1;295;0
WireConnection;322;0;320;0
WireConnection;323;0;322;0
WireConnection;323;1;324;0
WireConnection;325;0;327;0
WireConnection;316;1;315;0
WireConnection;317;0;316;0
WireConnection;320;0;317;0
WireConnection;320;1;321;0
WireConnection;327;0;326;0
WireConnection;327;1;323;0
WireConnection;70;0;105;0
WireConnection;136;0;132;0
WireConnection;252;1;204;0
WireConnection;0;2;136;0
WireConnection;0;11;252;0
ASEEND*/
//CHKSM=14E78583A5B5A1658182FF921F2E2C4889FB07BA