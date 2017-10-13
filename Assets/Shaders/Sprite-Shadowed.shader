Shader "Sprites/Sprite-Shadowed"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Geometry+1"
			"RenderType" = "Opaque"
			"ForceNoShadowCasting"="True"
			"CanUseSpriteAtlas" = "True"
		}

		LOD 200
		Offset -1, -1

		CGPROGRAM
		#pragma surface surf Standard decal:blend

		sampler2D _MainTex;
		fixed4 _Color;

		struct Input
		{
			float2 uv_MainTex;
			float4 color : COLOR;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color * IN.color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}

	Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}