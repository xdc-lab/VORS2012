float4x4 matViewProjection : register(c0);  
float4x4 matView : register(c4);  
float4   vecLightDir : register(c8);  
float4   vecEye : register(c12);

struct VS_INPUT   
{  
   float4 Position : POSITION0;  
   float3 Normal   : NORMAL0;  
   float2 Texcoord : TEXCOORD0;  
};  
  
struct VS_OUTPUT   
{  
   float4 Position : POSITION0;   
   float2 Texc     : TEXCOORD0;  
   float3 Light    : TEXCOORD1;  
   float3 Norm     : TEXCOORD2;  
   float3 View     : TEXCOORD3;      
};  

VS_OUTPUT main( VS_INPUT Input )
{
   VS_OUTPUT Output;

   Output.Position = mul( Input.Position, matViewProjection );  
   Output.Texc = Input.Texcoord;  
   Output.Light = vecLightDir;  
   Output.Norm = mul(Input.Normal, matView);  
     
   float3 posWorld = normalize(mul(Input.Position, matView));  
   Output.View = vecEye - posWorld;  
   
   return( Output );
   
}



