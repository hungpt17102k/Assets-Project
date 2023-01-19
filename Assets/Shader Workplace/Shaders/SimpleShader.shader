Shader "Unlit/SimpleShader"
{
    Properties
    {
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
            };

            // sampler2D _MainTex;
            // float4 _MainTex_ST;

            // Vertext shader
            VertexOutput vert(VertextInput v)
            {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normal = v.normal;
                o.clipSpacePos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(VertexOutput o) : SV_Target
            {
                float2 uv = o.uv0;

                float3 lightDir = normalize(float3(1, 1, 1));
                float lightFallOff = saturate(dot(lightDir, o.normal));
                float3 lightColor = float3(0.9, 0.82, 0.7);

                float3 diffuseLight = lightColor * lightFallOff;

                float3 ambientLight = float3( 0.2, 0.35, 0.4);
                
                return float4(ambientLight + diffuseLight, 0);
            }
            ENDCG
        }
    }
}
