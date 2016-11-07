Shader "Unlit/Road"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_PulseRate("Pulse Rate", Range(0.0, 10.0)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _PulseRate;
			float _BikeProgress;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				float intensity = 0.3f * col.r + 0.59f * col.g + 0.11f * col.b;
				intensity = pow(intensity, 3.0f) * 1.4f;

				const float pulseIntensity = 0.07f;
				intensity *= (1.0f - pulseIntensity) + sin(i.uv.x - _Time.b * _PulseRate) * pulseIntensity;

			//	intensity *= saturate(ceil(i.uv.x - _BikeProgress));

				fixed4 outCol = intensity * _Color;
				outCol.a = 0.5f;

				

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return outCol;
			}
			ENDCG
		}
	}
}
