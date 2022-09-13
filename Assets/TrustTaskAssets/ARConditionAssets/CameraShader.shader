Shader "TrustProject/CameraShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ARTex ("Texture", 2D) = "black" {}
        _MipLevel ("Mip level for resolution scaling", Range(0,6)) = 1
        _Opacity ("Opacity Slider",  Range(0.0, 1.0)) = 1.0
     }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _ARTex;
            float _MipLevel;
            float _Opacity;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float4 myfloat;
                myfloat.x = i.uv.x;
                myfloat.y = i.uv.y;
                myfloat.w = _MipLevel;
                fixed4 ARcol = tex2Dlod(_ARTex, myfloat) * _Opacity;

                // set the color to the AR image on non-black ar imagery
                col = (ARcol == fixed4(0,0,0,0)) ? col : ARcol;
                
                return col;
            }
            ENDCG
        }
    }
}
