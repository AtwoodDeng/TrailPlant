// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-7828-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32318,y:32748,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.3235294,c3:0.08255573,c4:1;n:type:ShaderForge.SFN_Tex2d,id:2811,x:32290,y:32952,ptovrint:False,ptlb:node_2811,ptin:_node_2811,varname:node_2811,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:08b38b7364d00a048b06b22d84ea6ce6,ntxv:0,isnm:False|UVIN-6073-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:6073,x:32068,y:32952,varname:node_6073,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:7416,x:32523,y:32865,varname:node_7416,prsc:2|A-7241-RGB,B-2811-RGB;n:type:ShaderForge.SFN_Tex2d,id:9523,x:32339,y:33192,ptovrint:False,ptlb:node_9523,ptin:_node_9523,varname:node_9523,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:9418344e067cb1e4286c800c1187d225,ntxv:0,isnm:False|UVIN-1047-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:5106,x:31755,y:33257,varname:node_5106,prsc:2;n:type:ShaderForge.SFN_Append,id:7948,x:31939,y:33275,varname:node_7948,prsc:2|A-5106-X,B-5106-Y;n:type:ShaderForge.SFN_Multiply,id:7828,x:32547,y:33090,varname:node_7828,prsc:2|A-7416-OUT,B-9523-RGB;n:type:ShaderForge.SFN_ValueProperty,id:2415,x:31919,y:33228,ptovrint:False,ptlb:Scale,ptin:_Scale,varname:node_2415,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.15;n:type:ShaderForge.SFN_Multiply,id:1047,x:32162,y:33228,varname:node_1047,prsc:2|A-7948-OUT,B-2415-OUT;proporder:7241-2811-9523-2415;pass:END;sub:END;*/

Shader "Shader Forge/stem" {
    Properties {
        _Color ("Color", Color) = (0,0.3235294,0.08255573,1)
        _node_2811 ("node_2811", 2D) = "white" {}
        _node_9523 ("node_9523", 2D) = "white" {}
        _Scale ("Scale", Float ) = 0.15
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _node_2811; uniform float4 _node_2811_ST;
            uniform sampler2D _node_9523; uniform float4 _node_9523_ST;
            uniform float _Scale;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _node_2811_var = tex2D(_node_2811,TRANSFORM_TEX(i.uv0, _node_2811));
                float2 node_1047 = (float2(i.posWorld.r,i.posWorld.g)*_Scale);
                float4 _node_9523_var = tex2D(_node_9523,TRANSFORM_TEX(node_1047, _node_9523));
                float3 emissive = ((_Color.rgb*_node_2811_var.rgb)*_node_9523_var.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
