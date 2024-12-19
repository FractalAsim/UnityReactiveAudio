Shader "Sound_VertexDisplacement_PatternColor"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SoundBufferTex("Sound Buffer Texture (R)", 2D) = "black" {}
		_AmpScale("Amplitude Scale", float) = 1.0
		_PatternTex("Pattern Texture (R)", 2D) = "white" {}
		_PatternThreshold("Pattern Color Threshold", float) = 1.0
		_PatternCol("Pattern Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 300
		//CULL OFF

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 normal: NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _PatternTex;
			float4 _PatternCol;
			float _PatternThreshold;
			sampler2D _SoundBufferTex;

			float4 _MainTex_ST;
			float _Amp[512];
			float _AmpScale;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);

				//sample the pattern texture RED value
				fixed pattern = tex2Dlod(_PatternTex, float4(o.uv,0,0)).r;

				//sample the soundbuffer using pattern.red as displacement amount * scale
				fixed displacementamt = tex2Dlod(_SoundBufferTex,1- pattern).r * _AmpScale;

				//apply threshold
				displacementamt *= max(0, sign((1 - pattern) - _PatternThreshold));

				//displace the vertex position by the normals and displacement amount
				o.vertex.xyz -= (v.normal * displacementamt);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				//sample the pattern texture RED value
				fixed pattern = tex2D(_PatternTex, i.uv).r;

				//if music texture input is 0 nullTestResult is 0 else it's 1
				//fixed nullTestData = saturate(pattern);
				//fixed nullTest = round(pattern);
				//fixed nullTestResult = lerp(0, 1, nullTestData);

				//sample sound buffer using the pattern
				//fixed snd = tex2D(_SoundBufferTex, float2((1- pattern), 0)).r*nullTestResult;
				fixed snd = tex2D(_SoundBufferTex, float2((1-pattern), 0)).r;
				//apply threshold
				snd *= max(0, sign((1-pattern) - _PatternThreshold));

				return col + snd * _PatternCol;
			}
			ENDCG
		}
	}
}