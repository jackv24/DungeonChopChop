Shader "Custom/FlashSurface" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_BumpMap("Normal", 2D) = "bump" {}
		_EmissionMap("Emission", 2D) = "black" {}
		_OcclusionMap("Occlusion", 2D) = "white" {}
		_FlashColor("Flash Color", Color) = (1,1,1,1)
		_FlashAmount("Flash Amount", Range(0.0,1.0)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _EmissionMap;
		sampler2D _OcclusionMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_EmissionMap;
			float2 uv_OcclusionMap;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _FlashColor;
		float _FlashAmount;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			c = lerp(c, _FlashColor, _FlashAmount);
			o.Albedo = c.rgb;

			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

			fixed4 e = tex2D(_EmissionMap, IN.uv_EmissionMap);
			e = lerp(e, _FlashColor, _FlashAmount);
			o.Emission = e.rgb;

			o.Occlusion = tex2D(_OcclusionMap, IN.uv_OcclusionMap);

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
