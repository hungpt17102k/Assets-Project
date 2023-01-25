Shader "Unlit/SimpleShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Gloss ("Gloss", Float) = 1

        // _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            // Mesh data: vertex position, vertex normal, UVs, tangents, vertex colors
            struct VertextInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv0 : TEXCOORD0;

                // float4 colors : COLOR;
                
                // float4 tangent : TANGENT;
                
                // float uv1 : TEXCOORD1;

            };

            struct VertexOutput
            {
                float4 clipSpacePos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            // sampler2D _MainTex;
            // float4 _MainTex_ST;

            float4 _Color;
            float _Gloss;
            uniform float3 _MousePos;

            // Vertext shader
            VertexOutput vert(VertextInput v)
            {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normal = v.normal;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.clipSpacePos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float InvLerp(float a, float b, float value)
            {
                return (value - a) / (b - a);
            }

            float3 MyLerp(float3 a, float3 b, float t) 
            {
                return t * b + (1.0 - t) * a;
            }

            float Posterize(float steps, float value)
            {
                return floor(value * steps) / steps;
            }

            float4 frag(VertexOutput o) : SV_Target
            {
                float dist = distance(_MousePos, o.worldPos);
                float glow = saturate(1 - dist);
                // return dist;

                float2 uv = o.uv0;
                float3 normal = normalize(o.normal); // Interpolated

                // Lighting

                // Direct diffuse light
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;

                // Direct diffuse light
                float lightFallOff = saturate(dot(lightDir, normal));
                // lightFallOff = Posterize(3, lightFallOff);
                float3 directDiffuseLight = lightColor * lightFallOff;

                // Ambient light
                float3 ambientLight = float3( 0.1, 0.1, 0.1);

                // Direct specular light
                float3 camPos = _WorldSpaceCameraPos;
                float3 fragToCam = camPos - o.worldPos;
                float3 viewDir = normalize(fragToCam);
                float3 viewReflect = reflect(-viewDir, normal);
                float specularFallOff = max(0, dot( viewReflect, lightDir));

                // Modify gloss
                specularFallOff = pow(specularFallOff, _Gloss);
                // specularFallOff = Posterize(3, specularFallOff);

                float directSpecular = specularFallOff * lightColor;

                // return float4(specularFallOff.xxx, 0);

                // Composite
                float3 diffuseLight = ambientLight + directDiffuseLight;
                float3 finalSurfaceColor = diffuseLight * _Color.rgb + directSpecular + glow;
                
                return float4(finalSurfaceColor, 0);
            }
            ENDCG
        }
    }
}
