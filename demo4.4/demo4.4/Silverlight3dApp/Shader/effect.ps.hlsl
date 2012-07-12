sampler2D basemap : register(s0);  

float4 main( float2 Texc:TEXCOORD0, 
			float3 Light:TEXCOORD1,           
			float3 Norm:TEXCOORD2, 
			float3 View:TEXCOORD3) : COLOR0  
{
   float4 ambient = { 1, 1, 1, 1};
   float4 diffuse = { 0.88f, 0.88f, 0.88f, 1.0f};  
     
   float3 Normal = normalize( Norm);  
   float3 LightDir = normalize( Light);  
   float3 ViewDir = normalize( View);  
   float4 diff = saturate( dot( Normal, LightDir));  
     
   float3 Reflect = normalize( 2 * diff * Normal - LightDir);  
   float4 specular = pow(saturate(dot(Reflect, ViewDir)), 8);  
     
   float4 fvBaseColor      = tex2D( basemap, Texc );  
   float4 fvTotalAmbient   = ambient * fvBaseColor;   
   float4 fvTotalDiffuse   = diffuse * diff * fvBaseColor;   
  
        
   return fvTotalAmbient + fvTotalDiffuse + specular;  
     
} 