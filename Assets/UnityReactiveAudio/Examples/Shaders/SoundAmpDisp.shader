Shader "SoundAmpDisp"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_SoundBufferTex("Sound Buffer Texture (R)", 2D) = "black" {}
	_AmpScale("Amplitude Scale", float) = 1.0
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100
		CULL OFF

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
		float4 color : TEXCOORD1;//store color
		float4 normal: NORMAL;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;

	sampler2D _SoundBufferTex;
	float _AmpScale;


	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);

		//sample sound buffer
		fixed4 col = tex2Dlod(_SoundBufferTex, o.uv.x);

		o.vertex.xyz -= (v.normal * col.r * _AmpScale);

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture
		fixed4 col = tex2D(_MainTex, i.uv);
	// apply fog
	//UNITY_APPLY_FOG(i.fogCoord, col);
	return col;
	}
		ENDCG
	}
	}
}