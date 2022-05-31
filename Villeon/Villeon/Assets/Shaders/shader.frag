#version 410 core

in vec4 fColor;
in vec2 fTexCoords;
in float fTexID;

uniform sampler2D textures[8];

out vec4 color;

void main(void)
{
	if (fTexID > 0)
	{
		int id = int(fTexID);
		vec4 texColor = fColor * texture(textures[id], fTexCoords);// * vec4(fTexCoords, 0, 1);
		color = texColor;
	}
	else
	{
		color = fColor;
	}
}