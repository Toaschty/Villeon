#version 420 core

#define MAX_POINT_LIGHTS 150

// Input
in vec4 fColor;
in vec2 fTexCoords;
in float fTexID;
uniform sampler2D textures[8];

// Output
out vec4 color;

void main(void)
{

	if (fTexID > 0)
	{
		// Get the textureColor and multiply it by the lightcolor 
		int id = int(fTexID);
        color = texture(textures[id], fTexCoords) * fColor;

	}
	else
	{
		// Draw only color
		color = fColor;
	}
}