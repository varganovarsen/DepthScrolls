Shader "Custom/ProceduralDirtSprite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}  // Required for sprite renderer
        _MainColor ("Main Color", Color) = (0.5, 0.3, 0.2, 1)
        _SecondaryColor ("Secondary Color", Color) = (0.6, 0.4, 0.25, 1)
        _NoiseScale ("Noise Scale", Range(0.1, 200)) = 15
        _RoughnessFactor ("Roughness", Range(0, 1)) = 0.8
        _BumpStrength ("Bump Strength", Range(0, 1)) = 0.3
        _DirtVariation ("Dirt Variation", Range(0, 1)) = 0.5
        _SmallNoiseInfluence ("Small Noise Influence", Range(0, 1)) = 0.4
        _OffsetX ("Offset X", Range(-20, 20)) = 0
        _OffsetY ("Offset Y", Range(-20, 20)) = 0
    }
    
    SubShader
    {
        Tags { 
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        
        struct Input
        {
            float2 uv_MainTex;  // Use UV coordinates for sprites
        };
        
        sampler2D _MainTex;  // Required for sprite renderer
        float _NoiseScale;
        float _RoughnessFactor;
        float _BumpStrength;
        float _DirtVariation;
        float _SmallNoiseInfluence;
        float _OffsetX;
        float _OffsetY;
        fixed4 _MainColor;
        fixed4 _SecondaryColor;
        
        // Hash function for noise generation
        float hash(float2 p)
        {
            p = 50.0 * frac(p * 0.3183099 + float2(0.71, 0.113));
            return -1.0 + 2.0 * frac(p.x * p.y * (p.x + p.y));
        }
        
        // 2D Noise function
        float noise(float2 p)
        {
            float2 i = floor(p);
            float2 f = frac(p);
            
            // Cubic interpolation
            float2 u = f * f * (3.0 - 2.0 * f);
            
            // Get noise values at corners
            float a = hash(i);
            float b = hash(i + float2(1.0, 0.0));
            float c = hash(i + float2(0.0, 1.0));
            float d = hash(i + float2(1.0, 1.0));
            
            // Interpolate
            return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
        }
        
        // Fractal Brownian Motion for more natural look
        float fbm(float2 p)
        {
            float value = 0.0;
            float amplitude = 0.5;
            float frequency = 1.0;
            
            for (int i = 0; i < 5; i++)
            {
                value += amplitude * noise(p * frequency);
                amplitude *= 0.5;
                frequency *= 2.0;
            }
            
            return value;
        }
        
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Apply offset to UV coordinates for the procedural pattern
            float2 offsetUV = IN.uv_MainTex + float2(_OffsetX, _OffsetY);
            
            // Generate multiple layers of noise using the offset UV coordinates
            float bigNoise = fbm(offsetUV * _NoiseScale * 0.1);
            float mediumNoise = fbm(offsetUV * _NoiseScale * 0.2);
            float smallNoise = fbm(offsetUV * _NoiseScale);
            
            // Combine noise patterns
            float combinedNoise = bigNoise * 0.5 + mediumNoise * 0.3 + smallNoise * _SmallNoiseInfluence;
            
            // Create dirt color variation with offset UVs
            float colorVariation = fbm(offsetUV * _NoiseScale * 0.3) * _DirtVariation;
            fixed4 dirtColor = lerp(_MainColor, _SecondaryColor, colorVariation);
            
            // Apply noise to the albedo
            o.Albedo = dirtColor.rgb + combinedNoise * 0.1;
            
            // Apply noise to normal for bumpy effect
            float3 normalNoise = float3(
                fbm(offsetUV * _NoiseScale * 1.1),
                fbm(offsetUV * _NoiseScale * 1.1 + 3.14),
                0
            );
            o.Normal = normalize(float3(0, 0, 1) + normalNoise * _BumpStrength);
            
            // Simulate roughness and specular
            o.Smoothness = 0.1 - fbm(offsetUV * _NoiseScale * 2.0) * _RoughnessFactor;
            o.Metallic = 0.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}