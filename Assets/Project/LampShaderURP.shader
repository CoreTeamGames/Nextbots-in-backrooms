Shader "Unlit/LampShaderURP"
{
   Properties
   {
       _BaseColor("Base Color", Color) = (1,1,1,1)
       _BaseMap("Base Texture", 2D) = "white" {}
       _EmissionColor("Emission Color", Color) = (1,1,1,1)
       _EmissionMap("Emission Texture", 2D) = "white" {}
       _Frequency("Frequency", Float) = 1.0
       _Amplitude("Amplitude", Float) = 0.5
   }
   SubShader
   {
       Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline" }
       LOD 100

       Pass
       {
           HLSLPROGRAM
           #pragma vertex vert
           #pragma fragment frag

           #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

           struct Attributes
           {
               float4 positionOS : POSITION;
               float2 uv : TEXCOORD0;
           };

           struct Varyings
           {
               float4 positionCS : SV_POSITION;
               float2 uv : TEXCOORD0;
           };

           sampler2D _BaseMap;
           float4 _BaseMap_ST;
           half4 _BaseColor;

           sampler2D _EmissionMap;
           half4 _EmissionColor;

           float _Frequency;
           float _Amplitude;

           Varyings vert (Attributes input)
           {
               Varyings output;
               output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
               output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
               return output;
           }

           half4 frag (Varyings input) : SV_Target
           {
               half4 baseColor = tex2D(_BaseMap, input.uv) * _BaseColor;

               // Calculate flickering effect
               float time = _Time.y * _Frequency;
               float flicker = sin(time) * _Amplitude;

               half4 emissionColor = _EmissionColor * tex2D(_EmissionMap, input.uv);
               emissionColor.rgb *= (1 + flicker);
               emissionColor = saturate(emissionColor);


               half4 finalColor = baseColor + emissionColor;

               return finalColor;
           }
           ENDHLSL
       }
   }
}
