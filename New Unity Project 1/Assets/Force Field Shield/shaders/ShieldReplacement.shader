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

Shader "KH/Distortion/ShieldReplacement"
{
	Properties
	{
		_MainTex ("Diffuse", 2D) = "grey"{}
	}
	SubShader
	{
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "ShieldEffect.cginc"
			struct vIN
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};
			
			struct vOUT
			{
				float4 pos : SV_POSITION;
				float3 oPos : TEXCOORD0;
				float3 wPos : TEXCOORD1;
				float3 wNorm : TEXCOORD2;
				float3 objPos : TEXCOORD3;
			};
			
			sampler2D _MainTex;
			

			vOUT vert(vIN v)
			{	
				vOUT o;
				o.pos = UnityObjectToClipPos(v.vertex);
				float3 zeroPos = UnityObjectToClipPos(float4(0.0,0.0,0.0,1.0));
				o.wPos = mul(unity_ObjectToWorld, v.vertex);				
				o.wNorm = normalize(mul(fixed4(v.normal, 0.0), unity_WorldToObject).xyz);
				o.oPos = normalize(o.pos.xyz - zeroPos.xyz);
				o.objPos = v.vertex.xyz;
				return o;
			}
			
			fixed4 frag(vOUT i) : COLOR
			{
				fixed4 tex = fixed4(i.oPos.x,0.0,i.oPos.y, 1.0);
			
				float intensity = CalcShieldIntensity16(i.objPos);
				
				float3 viewdir = normalize(_WorldSpaceCameraPos - i.wPos);
				float ang = 1.0- dot(viewdir, i.wNorm) + intensity;
				
				return (tex *  ang) + 0.5;
			}
			ENDCG
		}
	}
}