#version 420 core

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
		color *= fColor;
	}
	else
	{
		color = fColor;
	}
}