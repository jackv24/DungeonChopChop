Shader "Custom/SpinUVSurface" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Gradient("Gradient (Alpha)", 2D) = "white" {}
		_RotationSpeed("RotationSpeed", Float) = 1.0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Gradient;
		
		struct Input {
			float2 uv_MainTex;
			float2 uv_Gradient;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _RotationSpeed;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf(Input IN, inout SurfaceOutputStandard o) {
			IN.uv_MainTex.xy -= 0.5;

			float s = -sin(_Time.y * _RotationSpeed);
			float co = cos(_Time.y * _RotationSpeed);

			float2x2 rotationMatrix = float2x2(co, -s, s, co);
			rotationMatrix *= 0.5;
			rotationMatrix += 0.5;
			rotationMatrix = rotationMatrix * 2 - 1;

			float2 newUV = mul(IN.uv_MainTex.xy, rotationMatrix);

			float4 gradient = tex2D(_Gradient, IN.uv_Gradient);
			IN.uv_MainTex = lerp(IN.uv_MainTex, newUV, gradient.a);

			IN.uv_MainTex += 0.5;

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
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
