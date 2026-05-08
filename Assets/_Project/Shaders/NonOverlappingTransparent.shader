Shader "Custom/NonOverlappingTransparent"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,0.5)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
    }

    SubShader
    {
        // Transparent queue is correct, but we want to render after opaque objects
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200

        Pass {
            // Standard alpha transparency
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back // Change to Off if you want to see the inside, but Stencil will hide the back faces anyway

            Stencil {
                Ref 1
                Comp Always
                Pass Replace
                // If a pixel is already 1, we don't draw again
                Fail Keep
                ZFail Keep
            }

            // Standard logic: Only draw if the stencil value isn't 1 yet
            // This prevents overlapping parts of the same shader from stacking alpha
            Stencil {
                Ref 1
                Comp NotEqual
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _Color;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                // Premultiplied alpha logic if you prefer, 
                // but standard alpha works best with SrcAlpha OneMinusSrcAlpha
                return col;
            }
            ENDCG
        }
    }
}