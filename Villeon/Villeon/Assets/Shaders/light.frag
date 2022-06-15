#version 420 core

#define MAX_POINT_LIGHTS 2

out vec4 color;

in vec4 fColor;
in vec2 fTexCoords;
in float fTexID;
in vec3 fPosition;


struct BaseLight 
{
	vec3 color;
	float ambientIntensity;
};

struct DirectionalLight
{
	BaseLight baseLight;
	vec3 direction;
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

uniform vec3 normalVector;
uniform int lightCount;
uniform PointLight pointLights[MAX_POINT_LIGHTS];
uniform DirectionalLight directionalLight;

vec3 CalcLightInternal(BaseLight light, vec3 lightDirection, vec3 normal)
{
	vec3 ambientColor = light.color * light.ambientIntensity;
	return ambientColor; 
}

vec3 CalcDirectionalLight(vec3 normal)
{
	return CalcLightInternal(directionalLight.baseLight, directionalLight.direction, normal);
}

vec3 CalcPointLight(int index, vec3 normal)
{
	vec3 lightDirection = fPosition.xyz - pointLights[index].position;
	float distanceToLight = length(lightDirection);
	lightDirection = normalize(lightDirection);

	vec3 color = CalcLightInternal(pointLights[index].baseLight, lightDirection, normal);
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
		if (fPosition.z < -6)
		{
			int lights = lightCount;
			vec3 normal = vec3(0, 0, 1);

			completeLightColor = CalcDirectionalLight(normal);
			for (int i = 0; i < lights; i++)
			{
				completeLightColor += CalcPointLight(i, normal);
			}
		}


		// Get the textureColor
		int id = int(fTexID);
        color = texture(textures[id], fTexCoords) * vec4(completeLightColor, 1.0f);
	}
	else
	{
		color = fColor;
	}
}