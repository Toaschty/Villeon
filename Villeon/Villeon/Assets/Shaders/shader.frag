#version 420 core

#define MAX_LIGHTS 128
out vec4 color;


in vec4 fColor;
in vec2 fTexCoords;
in float fTexID;
uniform sampler2D textures[8];

void main(void)
{
	if (fTexID > 0)
	{
		// Get the textureColor
		int id = int(fTexID);
		color = texture(textures[id], fTexCoords);
	}
	else
	{
		color = fColor;
	}
}