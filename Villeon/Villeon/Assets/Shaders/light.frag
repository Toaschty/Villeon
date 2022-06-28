#version 420 core

#define MAX_POINT_LIGHTS 150

out vec4 color;

in vec4 fColor;
in vec2 fTexCoords;
in float fTexID;
in vec3 fPosition;

struct BaseLight 
{
	vec3 color;
	float intensity;
};

struct DirectionalLight
{
	BaseLight baseLight;
};

struct Attenuation
{
	float constant;
	float linear;
	float expo;
};

struct PointLight
{
	BaseLight baseLight;
	Attenuation attenuation;
	vec3 position;
};

uniform sampler2D textures[8];
uniform int lightCount;
uniform PointLight pointLights[MAX_POINT_LIGHTS];
uniform DirectionalLight directionalLight;

vec3 CalcLightInternal(BaseLight light)
{
	vec3 ambientColor = light.color * light.intensity;
	return ambientColor; 
}

vec3 CalcPointLight(int index)
{

	vec3 pixelPosition = vec3(round(fPosition.xy * 8) / 8, fPosition.z);
	float distanceToLight = length(pixelPosition - pointLights[index].position);

	vec3 color = CalcLightInternal(pointLights[index].baseLight);
	float attenuation = pointLights[index].attenuation.constant + 
						(pointLights[index].attenuation.linear * distanceToLight) +
						(pointLights[index].attenuation.expo * distanceToLight * distanceToLight);
	return color / attenuation;
}


void main(void)
{

	vec3 completeLightColor = vec3(1, 1, 1);

	// Only calculate light for non screen layers
	if (fPosition.z < -7)
	{
		int lights = lightCount;
		completeLightColor = CalcLightInternal(directionalLight.baseLight);
		for (int i = 0; i < lights; i++)
		{
			completeLightColor += CalcPointLight(i);
		}
	}

	// Cap the light 
	completeLightColor = min(completeLightColor, vec3(1.0, 1.0, 1.0));

	if (fTexID > 0)
	{
		// Get the textureColor
		int id = int(fTexID);
        color = texture(textures[id], fTexCoords) * vec4(completeLightColor, fColor.a) * fColor;
	}
	else
	{
		color = fColor * vec4(completeLightColor, fColor.a);
	}
}