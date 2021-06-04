// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Copyright (c) 2016 Kyle Halladay

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to 
//deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
//OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Shader "KH/Distortion/Shield" {
	Properties 
	{
		_Color("Color", Color) = (1,1,1,1)
		_RimColor("RimColor", Color) = (1, 1, 1, 1)
		_RimPower("RimPower", Range(0.0001, 4)) = 1 
		_RimIntensity("RimIntensity", Float) = 1
		_Speed("Speed", Float) = 1
		_MainTex ("Main Texture", 2D) = "white" {}
	
	}
	SubShader
	{ 
		Tags{"Queue" = "Transparent"}
		
	
		Pass 
		{
			LOD 200
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "ShieldEffect.cginc"
		
			fixed4 _Color, _RimColor;
			float _RimPower, _RimIntensity, _Speed;
			sampler2D _MainTex;			
			sampler2D _DistortionBuffer;
			sampler2D _ScreenBuffer;

			struct appdata_t 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f 
			{
				float4 vertex : SV_POSITION;
				float3 normal : TEXCOORD3;
				float2 texcoord : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				float3 oPos : TEXCOORD4;
			};
			
			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = mul(fixed4(v.normal, 0.0), unity_WorldToObject);
				o.texcoord = v.texcoord + half2(1, 0) * _Speed * _Time;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.screenPos = o.vertex;//ComputeScreenPos(o.vertex);
				o.oPos = v.vertex.xyz;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
//
				float3 viewdir = normalize(_WorldSpaceCameraPos - i.worldPos);
				float ang = 1 - (abs(dot(viewdir, normalize(i.normal))));
				half4 rimCol = _RimColor * pow(ang, _RimPower) * _RimIntensity;

				half4 texColor = tex2D(_MainTex, i.texcoord);
				fixed4 tex =  rimCol * texColor;
//				
				float4 screen = ComputeScreenPos(i.screenPos);
				fixed4 distortion = tex2Dproj(_DistortionBuffer, UNITY_PROJ_COORD(screen));
				
				float4 screenPos = screen;				
				screenPos.xy =  screenPos.xy - (distortion.rb - 0.5) * 1.0;

				float4 d = tex2Dproj(_ScreenBuffer, UNITY_PROJ_COORD(screenPos));
				return  tex + d + texColor * CalcShieldIntensity16(i.oPos);
			}
			ENDCG
		}		
		Pass 
		{
			LOD 200
			cull front
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "ShieldEffect.cginc"
		
			float _Speed;
			sampler2D _MainTex;			
			
			struct appdata_t 
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f 
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float3 oPos : TEXCOORD4;

			};
			
			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord + half2(1, 0) * _Speed * _Time;
				o.oPos = v.vertex.xyz;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				half4 texColor = tex2D(_MainTex, i.texcoord);
				texColor *= CalcShieldIntensity16(i.oPos);
				return texColor;
			}
			ENDCG
		}
	}
}