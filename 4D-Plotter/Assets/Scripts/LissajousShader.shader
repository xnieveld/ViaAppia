Shader "Custom/LissajousShader"
{
	Properties
	{

		_DiffuseColour("Diffuse Colour", Color) = (1.0, 1.0, 1.0, 1.0)
		_Scale("Scale", float) = 1
		_GridStep("Grid size", Float) = 10
		_GridWidth("Grid width", Float) = 1
		_GridOffsetX("X OffSet Grid", Float) = 0
		_GridOffsetY("Y OffSet Grid", Range(-100,100)) = 0
		_GridOffsetZ("Z OffSet Grid", Float) = 0
		_FigureNumber("Figure Number", Int) = 0


	}
		CGINCLUDE
#include "UnityCG.cginc"

	uniform float _FigureNumber;
	// PI = 3.14159265359;

	float4 lissajous0(int x, int y, int rows, int columns) {
		float t = (float)x / columns * 2 * 3.14159265359;
		float r = (float)y / rows * 2 * 3.14159265359;
		float4 newPoint = (0, 0, 0, 0);

		newPoint.x = (3 * cos(5.2 * t) + 1.7 * cos(26 * t)) * sin(r);
		newPoint.y = (3 * sin(5.2 * t) + 1.7 * sin(26 * t)) * sin(r);
		newPoint.z = (3 * sin(5.2 * t) + 1.7 * cos(26 * t)) * cos(r);

		return newPoint;
	}
	float4 lissajous1(int x, int y, int rows, int columns) {
		float t = (float)x / columns * 2 * 3.14159265359;
		float r = (float)y / rows * 2 * 3.14159265359;
		float4 newPoint = (0, 0, 0, 0);

		newPoint.x = (cos(t) * sin(r));
		newPoint.y = (sin(t) * sin(r));
		newPoint.z = (cos(r));
		return newPoint;
	}
	float4 lissajous2(int x, int y, int rows, int columns) {
		float t = (float)x / columns * 2 * 3.14159265359;
		float r = (float)y / rows * 2 * 3.14159265359;
		float4 newPoint = (0, 0, 0, 0);

		newPoint.x = (1.2 * sin(3.5 * t) * sin(r * 2));
		newPoint.y = (1.5 * cos(5 * t) * sin(r / 1.5));
		newPoint.z = (1.2 * sin(5.2 * t) * cos(r * 1.4));
		return newPoint;
	}

	float4 calculatePoint(int x, int y, int rows, int columns) {
		float4 newPoint = float4(0, 0, 0, 0);
		switch (_FigureNumber) {
		case 0:
			newPoint = lissajous0(x, y, rows, columns);
			break;
		case 1:
			newPoint = lissajous1(x, y, rows, columns);
			break;
		case 2:
			newPoint = lissajous2(x, y, rows, columns);
			break;
		default:
			newPoint = float4(((float)x - columns / 2) * 10 / columns, 0, ((float)y - rows / 2) * 10 / rows, 0);
			break;
		}

		return newPoint;
	}

	ENDCG
		SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Pass
	{
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

		float _RowCount;
	float _ColumnCount;
		float4 _DiffuseColour;
		float _Scale;
	half4 _Color;
	float _GridStep;
	float _GridWidth;
	float _GridOffsetX;
	float _GridOffsetY;
	float _GridOffsetZ;
	float3 _CamPos;

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0; //Contains row, colum possition of vertex
	};

	struct v2f
	{
		float3 worldPosition : POSITION3;
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
		float4 vertexOld : POSITION2;
	};

	v2f vert(appdata v)
	{
		v2f o;
		float4 vertex = calculatePoint(v.uv.x, v.uv.y, _RowCount, _ColumnCount);
		o.vertex = UnityObjectToClipPos(vertex);
		o.vertexOld = vertex;
		o.uv = v.uv;
		
		o.worldPosition = mul(unity_ObjectToWorld, vertex).xyz / _Scale;
		
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{

		float4 diffuseColour = _DiffuseColour;

		fixed4 c = (0,0,0,0);

		float2 offset = (_GridOffsetX, _GridOffsetZ);
		float2 pos = offset + float2(i.uv.x / _ColumnCount, i.uv.y / _RowCount) / (_GridStep * _Scale);
		float2 f = abs(frac(pos) - 0.5);
		float3 distance = sqrt(pow(_CamPos.x - i.worldPosition.x, 2) + pow(_CamPos.y - i.worldPosition.y, 2) + pow(_CamPos.z - i.worldPosition.z, 2)) / 1000;
		float2 df = fwidth(pos) * _GridWidth / distance;
		float2 g = round(smoothstep(-df, df, f) * 2) / 2;
		float grid = 1.0 - saturate(g.x * g.y);
		c.rgba = ceil(lerp(c.rgba, float4(1, 1, 1, 3), grid) / 10) * 10;

		c.a *= 0.05;
		
		float4 heightColor = (1, 1, 1, 1);
		heightColor.r = 1.0 - 1.0 / 1 * i.worldPosition.y;
		heightColor.g = 1.0 - 1.0 / 1 * i.worldPosition.y;

		float4 finalColor = (c * diffuseColour * heightColor);
		return finalColor;
	}
		ENDCG
	}

	}

}