Shader "Custom/StarrySky"
{
    Properties
    {
        _StarColor ("Star Color", Color) = (1,1,1,1)
        _BackgroundColor ("Background Color", Color) = (0,0,0.1,1)
        _StarDensity ("Star Density", Range(20, 100)) = 50
        _StarSize ("Star Size", Range(0.001, 0.01)) = 0.005
        _StarTwinkleSpeed ("Twinkle Speed", Range(0, 10)) = 1
        _StarBrightness ("Star Brightness", Range(0.1, 2)) = 1
        [Toggle] _EnableTwinkle ("Enable Twinkling", Float) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

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
                float facing : TEXCOORD1;
            };

            float4 _StarColor;
            float4 _BackgroundColor;
            float _StarDensity;
            float _StarSize;
            float _StarTwinkleSpeed;
            float _StarBrightness;
            float _EnableTwinkle;
            
            // Функция псевдослучайных чисел
            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898,78.233))) * 43758.5453123);
            }

            // Шум Перлина для более естественного мерцания
            float noise(float2 st)
            {
                float2 i = floor(st);
                float2 f = frac(st);
                
                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);

                return lerp(a, b, u.x) +
                        (c - a)* u.y * (1.0 - u.x) +
                        (d - b) * u.x * u.y;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                o.facing = dot(v.normal, viewDir);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Инвертируем UV-координаты для обратной стороны
                float2 uv = i.facing > 0 ? i.uv : float2(1 - i.uv.x, i.uv.y);
                uv = uv * _StarDensity;
                
                // Создаем сетку для звезд
                float2 gv = frac(uv) - 0.5;
                float2 id = floor(uv);
                
                float brightness = 0;
                
                // Проверяем каждую потенциальную позицию звезды
                for(float y = -1; y <= 1; y++)
                {
                    for(float x = -1; x <= 1; x++)
                    {
                        float2 offset = float2(x, y);
                        float2 p = id + offset;
                        
                        // Определяем, будет ли здесь звезда
                        float r = random(p);
                        if(r > 0.9) // Порог появления звезды
                        {
                            float2 pos = offset + gv;
                            float dist = length(pos);
                            
                            // Добавляем мерцание если оно включено
                            float starBrightness = 1;
                            if(_EnableTwinkle > 0)
                            {
                                float t = _Time.y * _StarTwinkleSpeed;
                                starBrightness = noise(p + t) * 0.5 + 0.5;
                            }
                            
                            // Создаем звезду с мягкими краями
                            brightness += smoothstep(_StarSize, 0, dist) * starBrightness * _StarBrightness;
                            
                            // Добавляем свечение вокруг звезды
                            float glow = smoothstep(_StarSize * 3.0, 0, dist) * 0.3 * starBrightness * _StarBrightness;
                            brightness += glow;
                        }
                    }
                }
                
                // Смешиваем цвет звезд с фоном
                fixed4 col = lerp(_BackgroundColor, _StarColor, brightness);
                return col;
            }
            ENDCG
        }
    }
}
