Shader "Unlit/NewUnlitShader"
{
	Properties
	{
		_MainTex ("Texture A (RGB)", 2D) = "black" {}
        _TextureB ("Texture B (RGB)", 2D) = "black" {}
        _TextureC ("Texture C (RGB)", 2D) = "black" {}
        _FlowSpeedA ("Flow Speed A (UV X, UV Y)", VECTOR) = (0, 0, 0, 0)
        _FlowSpeedB ("Flow Speed B (UV X, UV Y)", VECTOR) = (0, 0, 0, 0)
        _FlowSpeedC ("Flow Speed C (UV X, UV Y)", VECTOR) = (0, 0, 0, 0)
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 200
        ZWrite Off

			CGPROGRAM
        	#pragma surface surf Lambert alpha
 
        	sampler2D _MainTex;
        	sampler2D _TextureB;
        	sampler2D _TextureC;
        	float4 _FlowSpeedA;
        	float4 _FlowSpeedB;
        	float4 _FlowSpeedC;
      
 
        	struct Input
        	{
            	float2 uv_MainTex;
            	float2 uv_TextureB;
            	float2 uv_TextureC;
        	};
 
        	void surf (Input i, inout SurfaceOutput o)
        	{
            	fixed4 srcA = tex2D(_MainTex,  i.uv_MainTex  + _FlowSpeedA.xy * _Time.x);
            	fixed4 srcB = tex2D(_TextureB, i.uv_TextureB + _FlowSpeedB.xy * _Time.x);      
            	fixed4 srcC = tex2D(_TextureC, i.uv_TextureC + _FlowSpeedC.xy * _Time.x);
          
            	fixed4 col = fixed4(
                	srcA.rgb * srcA.a + srcB.rgb * srcB.a * (1 - srcA.a),
                	srcA.a + srcB.a * (1 - srcA.a));
              
            	fixed4 colB = fixed4(
                	col.rgb * col.a + srcC.rgb * srcC.a * (1 - col.a),
                	col.a + srcC.a * (1 - col.a));
              
            	o.Albedo = colB.rgb;
            	o.Alpha = colB.a;
        	}
			ENDCG
	}
	FallBack "Transparent/VertexLit"
}
