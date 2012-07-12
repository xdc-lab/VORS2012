float4x4 WorldViewProj : register(c0);
float4x4 World : World : register(c4);
float3 Light0 : TEXCOORD : register(c8);

float3 LightDirection : Direction = float3(0,100,100);
//float3 CameraPosition : CameraPosition;

// vertex input to the shader
struct VS_IN
{
	float4 Position : POSITION;
	float2 UV : TEXCOORD;
	float3 Normal : NORMAL;
};

struct VS_OUT
{
	float4 Position : POSITION;
	float2 UV : TEXCOORD;
	float3 Light : TEXCOORD1;
	float3 Normal : TEXCOORD2;
};

VS_OUT main(VS_IN input)
{
	 VS_OUT output = (VS_OUT)0;

	 output.Position = mul(input.Position, WorldViewProj);
	 output.UV = input.UV;
	 output.Light = normalize(Light0 - input.Position);
	 output.Normal = mul(input.Normal, World);

	 return output;
}
