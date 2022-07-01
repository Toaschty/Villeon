#version 410 core
#define MAX_POINT_LIGHTS 150

// Input
layout(location = 0) in vec4 position;
layout(location = 1) in vec4 color;
layout(location = 2) in vec2 texCoords;
layout(location = 3) in float texID;
uniform mat4 screenMatrix;
uniform mat4 cameraMatrix;


// Output
out vec4 fColor;
out vec2 fTexCoords;
out float fTexID;


//////////////////// Light  /////////////////////
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

// Input
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
	float distanceToLight = length(position.xyz - pointLights[index].position);

	vec3 color = CalcLightInternal(pointLights[index].baseLight);
	float attenuation = pointLights[index].attenuation.constant + 
						(pointLights[index].attenuation.linear * distanceToLight) +
						(pointLights[index].attenuation.expo * distanceToLight * distanceToLight);
	return color / attenuation;
}

////////////////////////////////////////////

void main()
{
	// This will be the lightcolor
    fColor = color;

	// Texturing
	fTexCoords = texCoords;
	fTexID = texID;

	// Screen Layers: -0, -1, -2, -3, -4, -5, -6, -7
	if (position.z >= -7 )
	{
		// Don't calculate lighting for UI elements on the ScreenLayer
		gl_Position = position * screenMatrix;
	}

	// everything after the screen layers uses the camera
	if (position.z < -7)
	{
		vec3 lightColor = vec3(1, 1, 1);
		int lights = lightCount;

		lightColor = CalcLightInternal(directionalLight.baseLight);

		for (int i = 0; i < lights; i++)
		{
			lightColor += CalcPointLight(i);
		}

		// Cap the lightColor to a max brightness
		lightColor = min(lightColor, vec3(1.0, 1.0, 1.0));

		// Apply the wanted color to the lightcolor
		fColor = vec4(lightColor, color.a) * color;

		gl_Position = position * cameraMatrix;
	}
}