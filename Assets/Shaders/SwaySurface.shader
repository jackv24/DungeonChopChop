Shader "Custom/SwaySurface" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_SwaySpeed("Sway Speed", Float) = 1.0
		_SwayMagnitude ("Sway Magnitude", Float) = 1.0
		_HeightOffset("Height Sway Offset", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _SwaySpeed;
		float _SwayMagnitude;
		float _HeightOffset;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void vert(inout appdata_full v)
		{
			//Get vertex world coordinates
			float4 worldV = mul(unity_ObjectToWorld, v.vertex);

			float height = sqrt(worldV.y * worldV.y);
			float offset = sin(_Time.y * _SwaySpeed + ((worldV.x + worldV.z) / 2) + height * _HeightOffset);

			//Move world vertex position
			worldV += float4(offset * height * _SwayMagnitude, 0.0f, offset * height * _SwayMagnitude, 0.0f);

			//Set object vertex pos
			v.vertex = mul(unity_WorldToObject, worldV);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
