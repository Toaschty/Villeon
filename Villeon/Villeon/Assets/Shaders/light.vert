#version 410 core

#define MAX_LIGHTS 128

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 color;
layout(location = 2) in vec2 texCoords;
layout(location = 3) in float texID;

uniform mat4 cameraMatrix;
uniform mat4 screenMatrix;

out vec4 fColor;
out vec2 fTexCoords;
out float fTexID;

struct DirLight {
    vec3 direction;
    vec3 ambient; // Color
    vec3 diffuse; // Color
};

struct PointLight {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
};

// calculates the color when using a point light.
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 pos)
{
    vec3 lightDir = normalize(light.position - pos);

    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);

    // attenuation
    float constant = 1.0f;
    float linear = 2f;
    float quadratic = 10f;

    float distance = length(light.position - pos);
    float attenuation = 1.0 / (constant + linear * distance + quadratic * (distance * distance));  
    
    // combine results
    vec3 ambient = light.ambient;
    vec3 diffuse = light.diffuse * diff;
    ambient *= attenuation;
    diffuse *= attenuation;
    return (ambient + diffuse);
}

// calculates the color when using a directional light.
vec3 CalcDirLight(DirLight light, vec3 normal)
{
    vec3 lightDir = normalize(-light.direction);

    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);

    // combine results
    vec3 ambient = light.ambient;
    vec3 diffuse = light.diffuse * diff;
    return (ambient + diffuse);
}

uniform vec3 normalVector;
uniform int lightCount;

uniform DirLight dirLight;
uniform PointLight pointLights[MAX_LIGHTS];

// fucntion declarations
vec3 CalcDirLight(DirLight light, vec3 normal);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 pos);

void main()
{
    fColor = color;
    if (position.z < -6)
    {
        vec3 normal = normalize(normalVector);
        int lights = lightCount;

        vec3 resultColor = CalcDirLight(dirLight, normal);
        for (int i = 0; i < lights; i++)
        {
            resultColor += CalcPointLight(pointLights[i], normal, position.xyz) * 500;
        }

	    fColor = vec4(resultColor, 1f);
    }

	fTexCoords = texCoords;
	fTexID = texID;

	if (position.z < -2)
	{
		gl_Position = position * cameraMatrix;
	}

	if (position.z >= -2 )
	{
		gl_Position = position * screenMatrix;
	}
}