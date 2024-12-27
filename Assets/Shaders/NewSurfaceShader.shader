// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "2D/Texture Blend"
{  
    Properties
    {
       _MainTex ("Sprite Texture", 2D) = "white" {}
       _Color ("Alpha Color Key", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "Queue" = "Transparent+1" 
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha 
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON
 
            sampler2D _MainTex;
            float4 _Color;

            struct Vertex
            {
                float4 vertex : POSITION;
                float2 uv_MainTex : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };
    
            struct Fragment
            {
                float4 vertex : POSITION;
                float2 coords: POSITION1;
                float2 uv_MainTex : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };
 
            Fragment vert(Vertex v)
            {
                Fragment o;
    
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.coords = v.vertex;
                o.uv_MainTex = v.uv_MainTex;
                o.uv2 = v.uv2;
    
                return o;
            }
                                                    
            float4 frag(Fragment IN) : COLOR
            {
                float4 o = float4(1, 0, 0, 0.2);

                half4 c = tex2D(_MainTex, IN.uv_MainTex);
                float2 pos = IN.uv_MainTex;
                o.rgb = c.rgb;
                o.a = 1;
                if (IN.uv_MainTex.x > 0.5) o.a = 0;
                // if(c.r == _Color.r && c.g == _Color.g && c.b == _Color.b)
                // {
                //     o.a = 0;
                // }
                // else
                // {
                //     o.a = 1;
                // }
                    
                return o;
            }

            ENDCG
        }
    }
}