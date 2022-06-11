#version 420 core

#define MAX_LIGHTS 128
out vec4 color;

struct DirLight {
    vec3 direction;
	
    vec3 ambient; // Color
    vec3 diffuse; // Color
};

struct PointLight {
    vec3 position;
    
    float constant;
    float linear;
    float quadratic;
	
    vec3 ambient;
    vec3 diffuse;
};


in vec4 fColor;
in vec2 fTexCoords;
in float fTexID;
in vec3 fQuadPosition;
in int doLighting;

uniform sampler2D textures[8];
uniform vec3 normalVector;
uniform int lightCount;

uniform DirLight dirLight;
uniform PointLight pointLights[MAX_LIGHTS];

// calculates the color when using a point light.
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, int id, vec3 texColor)
{
    vec3 lightDir = normalize(light.position - fragPos);

    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);

    // attenuation
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));  
    
    // combine results
    vec3 ambient = light.ambient * texColor;
    vec3 diffuse = light.diffuse * diff * texColor;
    ambient *= attenuation;
    diffuse *= attenuation;
    return (ambient + diffuse);
}

// calculates the color when using a directional light.
vec3 CalcDirLight(DirLight light, vec3 normal, int id, vec3 texColor)
{
    vec3 lightDir = normalize(-light.direction);

    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);

    // combine results
    vec3 ambient = light.ambient * texColor;
    vec3 diffuse = light.diffuse * diff * texColor;
    return (ambient + diffuse);
}

// fucntion declarations
vec3 CalcDirLight(DirLight light, vec3 normal, int id, vec3 texColor);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, int id, vec3 texColor);

void main(void)
{
	vec3 normal = normalize(normalVector);

	// make sure to not go above the lightcount
	int lights = lightCount;
	if (lightCount >= MAX_LIGHTS)
		lights = 0;

	if (fTexID > 0)
	{
		// Get the textureColor
		int id = int(fTexID);
		
        vec4 texColor = texture(textures[id], fTexCoords);
        vec3 resultColor = CalcDirLight(dirLight, normal, id, texColor.xyz);
        for (int i = 0; i < lights; i++)
        {
            resultColor += CalcPointLight(pointLights[i], normal, fQuadPosition, id, texColor.xyz);
        }
		color = vec4(resultColor, texColor.a);
	}
	else
	{
		color = fColor;
	}
}