#version 410 core

in vec4 fColor;
in vec2 fTexCoords;
in float fTexID;
in vec3 fQuadPosition;

uniform sampler2D textures[8];
uniform vec4 lightColor;
uniform vec3 lightPosition;
uniform vec3 normalVector;

out vec4 color;

void main(void)
{

	vec3 normal = normalize(normalVector);
	vec3 lightDirection = normalize(lightPosition - fQuadPosition);

	float ambientLight = 0.2f;
	float diffuse = max(dot(normal, lightDirection), 0.0f);

	if (fTexID > 0)
	{
		int id = int(fTexID);
		vec4 texColor = texture(textures[id], fTexCoords) * lightColor * (diffuse + ambientLight);
		color = texColor;
	}
	else
	{
		color = fColor;
	}
}