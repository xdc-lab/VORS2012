float4x4 WorldViewProj : register(c0);

float AmbientIntensity = 0.1;
float4 AmbientColor = float4(1,1,1,1);
float4 SpecularColor = float4(1, 1, 1, 1);    
float SpecularIntensity = 1;
float shininess = 200;

sampler Outline;

texture textureMap : register(t0);
sampler textureSampler = sampler_state
{
	texture = <textureMap>;
};

struct VS_OUT
{
	float4 Position : POSITION;
	float2 UV : TEXCOORD;
	float3 Light : TEXCOORD1;
	float3 Normal : TEXCOORD2;
};

struct PS_OUT
{
	float4 Color : COLOR;
};

PS_OUT main(VS_OUT input)
{  
	PS_OUT output = (PS_OUT)0;

	float3 LightDir = normalize(input.Light);
	float Diffuse = saturate(dot(LightDir, normalize(input.Normal)));
	float4 texCol = tex2D(textureSampler, input.UV);
	float4 Ambient = AmbientIntensity * AmbientColor;
	//texCol *= Diffuse;
	output.Color = Ambient + texCol;

	return output;
}
