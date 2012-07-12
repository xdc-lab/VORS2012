texture Source;
float Kernel[13] = {-6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6};

static const float Weights[13] = { 0.002216, 0.008764, 0.026995, 0.064759, 0.120985, 0.176033, 0.199471, 0.176033, 0.120985, 0.064759, 0.026995, 0.008764, 0.002216};

sampler2D postTex = sampler_state
{
        texture         = <Source>;
        MinFilter       = Linear;
        MagFilter       = Linear;
        MipFilter       = Linear;
};

void main(in float2 tex : TEXCOORD0, out float4 dif : COLOR0)
{
        dif = float4(0.0, 0.0, 0.0, 0.0);
        float2 coord;

        coord.y = tex.y;

        for(int i = 0; i < 13; ++i)
        {
                coord.x = tex.x + Kernel[i];
                dif += tex2D(postTex, coord.xy) * Weights[i];
        }
}