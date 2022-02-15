Shader "Unlit/Cover Mask"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Mask("Mask",2D)="white" {}
		_Alpha("Alpha", float) = 1
	}
	SubShader
	{
		// No culling or depth
		Tags {"Queue"="Transparent"
		"IgnoreProjector"="True" }
		Cull Off
		ZWrite Off
		ZTest Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			sampler2D _Mask;
			fixed4 _MainTex_ST;
			float _Alpha;
			struct appdata
			{
				half4 vertex : POSITION;
				half2 uv : TEXCOORD0;
			};

			struct v2f
			{
				half2 uv : TEXCOORD0;
				half4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv,_MainTex);
				return o;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				half4 cd =tex2D(_MainTex, i.uv);
				half4 ma=tex2D(_Mask, i.uv);
				cd.a=ma.a * _Alpha;
				return cd;
			}
			ENDCG
		}
	}
}