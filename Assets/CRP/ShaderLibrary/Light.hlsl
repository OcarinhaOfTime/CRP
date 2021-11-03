#ifndef CUSTOM_LIGHT_INCLUDED
#define CUSTOM_LIGHT_INCLUDED

#define MAX_DIRECTIONAL_LIGHT_COUNT 4

CBUFFER_START(_CustomLight)
	int _DirectionalLightCount;
	float3 _DirectionalLightColors[MAX_DIRECTIONAL_LIGHT_COUNT];
	float3 _DirectionalLightDirections[MAX_DIRECTIONAL_LIGHT_COUNT];
	float4 _DirectionalLightShadowData[MAX_DIRECTIONAL_LIGHT_COUNT];
CBUFFER_END

struct Light {
	float3 color;
	float3 direction;
	float attenuation;
};

int GetDirectionalLightCount () {
	return _DirectionalLightCount;
}

DirectionalShadowData GetDirectionalShadowData(int lightIndex){
	DirectionalShadowData data;
	float2 data_v = _DirectionalLightShadowData[lightIndex];

	data.strength = data_v.x;
	data.tileIndex = data_v.y;

	return data;
}

Light GetDirectionalLight (int i, Surface surfaceWS) {
	Light light;
	light.color = _DirectionalLightColors[i];
	light.direction = _DirectionalLightDirections[i];

	DirectionalShadowData shadowData = GetDirectionalShadowData(i);
	light.attenuation = GetDirectionalShadowAttenuation(shadowData, surfaceWS);

	return light;
}



#endif