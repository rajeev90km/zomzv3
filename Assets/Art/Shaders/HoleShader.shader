﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Zomz/Hole" {
 
 Properties {
     _Center ("Hole Center", Vector) = (.5, .5, 0 , 0)
     _Radius ("Hole Radius", Float) = .25
     _Shape ("Hole Shape", Float) = .25
     _Color ("Main Color", Color) = (1,1,1,1)
     _MainTex ("Main Texture", 2D) = ""
 }
 
 SubShader {
     Tags {"Queue" = "Transparent"}
     Blend SrcAlpha OneMinusSrcAlpha
     Pass {
         CGPROGRAM
         struct appdata {
             float4 position : POSITION;
             half2 texCoord : TEXCOORD;
         }; 
         
         struct v2f {
             float4 position_clip : SV_POSITION;
             half2 position_uv : TEXCOORD;
         };
         
         #pragma vertex vert
         uniform half4 _MainTex_ST;
         uniform float4 _Color_ST;
         v2f vert(appdata i) {
             v2f o;
             o.position_clip = UnityObjectToClipPos(i.position);
             o.position_uv = _MainTex_ST.xy * i.texCoord + _MainTex_ST.zw;
             return o;
         }
         
         #pragma fragment frag
         uniform sampler2D _MainTex;
         float4 _Color;
         uniform half2 _Center;
         half _Radius, _Shape;
         fixed4 frag(v2f i) : COLOR {        
             fixed4 fragColor = _Color;
             half hole = min(distance(i.position_clip, _Center) / _Radius, 1.);
             fragColor.a *= pow(hole, _Shape);
             return fragColor;
         }
         ENDCG
     }
 }
 
 }