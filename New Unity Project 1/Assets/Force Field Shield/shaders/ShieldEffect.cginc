//Copyright (c) 2016 Kyle Halladay

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to 
//deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
//OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

float4x4 _Positions0;
float4 _Intensities0;
float4 _Radius0;

float4x4 _Positions1;
float4 _Intensities1;
float4 _Radius1;

float4x4 _Positions2;
float4 _Intensities2;
float4 _Radius2;

float4x4 _Positions3;
float4 _Intensities3;
float4 _Radius3;

float calcIntensity0(float3 oPos)
{			
	float3 impact0 = (_Positions0[0].xyz - oPos);
	float3 impact1 = (_Positions0[1].xyz - oPos);
	float3 impact2 = (_Positions0[2].xyz - oPos);
	float3 impact3 = (_Positions0[3].xyz - oPos);
	
	float4 sqrLens = float4(dot(impact0,impact0),
	 						dot(impact1,impact1), 
	 						dot(impact2,impact2), 
	 						dot(impact3,impact3));
							 
	float4 cmpRad = _Radius0 > sqrLens;
	float4 add = (sqrLens) * cmpRad * _Intensities0;
	float intensity = add.x + add.y + add.z + add.w;
	return intensity*intensity;
}

float calcIntensity1(float3 oPos)
{			
	float3 impact0 = (_Positions1[0].xyz - oPos);
	float3 impact1 = (_Positions1[1].xyz - oPos);
	float3 impact2 = (_Positions1[2].xyz - oPos);
	float3 impact3 = (_Positions1[3].xyz - oPos);
	
	float4 sqrLens = float4(dot(impact0,impact0),
	 						dot(impact1,impact1), 
	 						dot(impact2,impact2), 
	 						dot(impact3,impact3));
							 
	float4 cmpRad = _Radius1 > sqrLens;
	float4 add = sqrLens * cmpRad * _Intensities1;
	float intensity = add.x + add.y + add.z + add.w;
	return intensity*intensity;
}

float calcIntensity2(float3 oPos)
{			
	float3 impact0 = (_Positions2[0].xyz - oPos);
	float3 impact1 = (_Positions2[1].xyz - oPos);
	float3 impact2 = (_Positions2[2].xyz - oPos);
	float3 impact3 = (_Positions2[3].xyz - oPos);
	
	float4 sqrLens = float4(dot(impact0,impact0),
	 						dot(impact1,impact1), 
	 						dot(impact2,impact2), 
	 						dot(impact3,impact3));
							 
	float4 cmpRad = _Radius2 > sqrLens;
	float4 add = sqrLens * cmpRad * _Intensities2;
	float intensity = add.x + add.y + add.z + add.w;
	return intensity*intensity;
}

float calcIntensity3(float3 oPos)
{			
	float3 impact0 = (_Positions3[0].xyz - oPos);
	float3 impact1 = (_Positions3[1].xyz - oPos);
	float3 impact2 = (_Positions3[2].xyz - oPos);
	float3 impact3 = (_Positions3[3].xyz - oPos);
	
	float4 sqrLens = float4(dot(impact0,impact0),
	 						dot(impact1,impact1), 
	 						dot(impact2,impact2), 
	 						dot(impact3,impact3));
							 
	float4 cmpRad = _Radius3 > sqrLens;
	float4 add = sqrLens * cmpRad * _Intensities3;
	float intensity = add.x + add.y + add.z + add.w;
	return intensity*intensity;
}

float CalcShieldIntensity4(float3 oPos)
{
	return calcIntensity0(oPos);
}

float CalcShieldIntensity8(float3 oPos)
{
	return calcIntensity0(oPos) + calcIntensity1(oPos);
}

float CalcShieldIntensity12(float3 oPos)
{
	return calcIntensity0(oPos) + calcIntensity1(oPos) + calcIntensity2(oPos);
}

float CalcShieldIntensity16(float3 oPos)
{
	return calcIntensity0(oPos) + calcIntensity1(oPos) + calcIntensity2(oPos) + calcIntensity3(oPos);
}

