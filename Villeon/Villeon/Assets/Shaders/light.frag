#version 420 core

#define MAX_POINT_LIGHTS 64

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
	if (fTexID > 0)
	{
		vec3 completeLightColor = vec3(1, 1, 1);
		if (fPosition.z < -2)
		{
			int lights = lightCount;
			completeLightColor = CalcLightInternal(directionalLight.baseLight);
			for (int i = 0; i < lights; i++)
			{
				completeLightColor += CalcPointLight(i);
			}
		}

		// Cap the light 
		completeLightColor = min(completeLightColor, vec3(1.2, 1.2, 1.2));

		// Get the textureColor
		int id = int(fTexID);
        color = texture(textures[id], fTexCoords) * vec4(completeLightColor, 1.0f);
	}
	else
	{
		color = fColor;
	}
}