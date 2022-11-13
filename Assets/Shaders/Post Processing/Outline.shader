Shader "Hidden/Custom/Outline"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_OutlineMask("_OutlineMask", 2D) = "clear" {}
		_OverlayScale("Overlay Scale", float) = 1.0
		_OverlayTint("Overlay Tint", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

		Pass
		{
			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

			#pragma vertex vert
			#pragma fragment frag

			TEXTURE2D(_MainTex);
			TEXTURE2D(_OutlineMask);

			SAMPLER(sampler_MainTex);
			SAMPLER(sampler_OutlineMask);

			float4 _MainTex_TexelSize;

			float _OverlayScale;
			float4 _OverlayTint;

			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Varyings
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
				output.vertex = vertexInput.positionCS;
				output.uv = input.uv;

				return output;
			}

			float ApplyStripes (float input, float2 uv)
			{
				float ss = uv.x + uv.y;
				ss *= 10;
				ss += _Time[1];
				ss %= 1;

				return (ss > 0.5) * input;
			}

			float4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float mask = 0.0;
				int expand = 5;
				for (int i = -expand; i <= expand; i++)
				{
					for (int j = -expand; j <= expand; j++)
					{
						float2 uv = input.uv + (float2(i, j) * _MainTex_TexelSize.xy);
						mask += SAMPLE_TEXTURE2D(_OutlineMask, sampler_OutlineMask, uv).r;
					}
				}
				mask = min(mask, 1);
				mask -= SAMPLE_TEXTURE2D(_OutlineMask, sampler_OutlineMask, input.uv).r;

				mask = ApplyStripes(mask, input.uv);

				float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
				return lerp(color, mask, mask.r);
			}

			ENDHLSL
		}
	}

	FallBack "Diffuse"
}