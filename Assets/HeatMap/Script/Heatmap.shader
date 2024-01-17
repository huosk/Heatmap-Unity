Shader "Custom/Heatmap" {
	Properties{
		_HeatTex("Texture", 2D) = "white" {}
	}

	SubShader{
		Tags{ "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha // Alpha blend

		Pass{
			CGPROGRAM
			#pragma vertex vert             
			#pragma fragment frag

			struct vertInput {
				float4 pos : POSITION;
			};

			struct vertOutput {
				float4 pos : SV_POSITION;
				half3 worldPos : TEXCOORD1;
			};

			struct PointInfo {
				float3 position;	// (x, y, z) = world space position
				float radius;		// radius
				float intensity;	// intensity
			};

			StructuredBuffer<PointInfo> _PointBuffer;
			int _Points_Length = 0;

			sampler2D _HeatTex;

			vertOutput vert(vertInput input) {
				vertOutput o;
				o.pos = UnityObjectToClipPos(input.pos);
				o.worldPos = mul(unity_ObjectToWorld,input.pos);
				return o;
			}

			float Blob(float3 position,float3 center, float radius)
			{
				float dis = distance(position,center);
				float result = 0.0;
				if( dis < radius)
				{		
					float f = dis / radius;		
					result = pow((1.0-pow(f,2.0)),2.0);		
				}
				return result;
			}

			half4 frag(vertOutput output) : SV_TARGET
			{
				float blobValue = 0.0;

				for (int i = 0; i < _Points_Length; i++)
				{
					float blob = Blob(output.worldPos.xyz, _PointBuffer[i].position, _PointBuffer[i].radius) * _PointBuffer[i].intensity;
					blobValue += blob;
				}

				half4 color = tex2D(_HeatTex,fixed2(blobValue,0.5));
				return color;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}