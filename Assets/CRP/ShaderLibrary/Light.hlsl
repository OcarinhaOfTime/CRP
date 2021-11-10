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

DirectionalShadowData GetDirectionalShadowData(int lightIndex, ShadowData shadowData){
	DirectionalShadowData data;
	float3 data_v = _DirectionalLightShadowData[lightIndex];

	data.strength = data_v.x * shadowData.strength;
	data.tileIndex = data_v.y + shadowData.cascadeIndex;
	data.normalBias = data_v.z;

	return data;
}

Light GetDirectionalLight (int i, Surface surfaceWS, ShadowData shadowData) {
	Light light;
	light.color = _DirectionalLightColors[i];
	light.direction = _DirectionalLightDirections[i];

	DirectionalShadowData dirShadowData = GetDirectionalShadowData(i, shadowData);
	light.attenuation = GetDirectionalShadowAttenuation(dirShadowData, shadowData, surfaceWS);
	//light.attenuation = shadowData.cascadeIndex * 0.25;

	return light;
}



#endif