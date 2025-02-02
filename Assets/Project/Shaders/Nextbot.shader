Shader "Custom/Nextbot"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {} 
        _FadeStart ("Fade Start Distance", Float) = 10.0
        _FadeEnd ("Fade End Distance", Float) = 20.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            // Включаем прозрачность
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _FadeStart;
            float _FadeEnd;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float  dist   : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                // Получаем мировую позицию вершины
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // Вычисляем расстояние от камеры до вершины
                o.dist = distance(_WorldSpaceCameraPos, worldPos.xyz);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Линейное растворение:
                // При расстоянии <= _FadeStart fade = 1, при >= _FadeEnd fade = 0.
                float fade = saturate((_FadeEnd - i.dist) / (_FadeEnd - _FadeStart));
                col.a *= fade;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}