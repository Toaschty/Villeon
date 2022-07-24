#version 420 core

out vec4 color;

in vec4 fColor;
in vec2 fTexCoords;
in float fTexID;
in vec3 fPosition;

uniform sampler2D textures[9];
uniform float dimensions[2];

void main(void)
{
	vec4 completeLightColor = vec4(min(1.0, dimensions[0]));

	if (fPosition.z < -7)
	{
		vec2 lightMapPosition = gl_FragCoord.xy / vec2(dimensions[0], dimensions[1]);
		completeLightColor = texture(textures[8], lightMapPosition);
		int radius = 3;
		int count = 1;
		for(int x = -radius; x <= radius; x++)
		{
			for(int y = -radius; y <= radius; y++)
			{
				count++;
				completeLightColor += texture(textures[8], lightMapPosition + vec2(x / 256.0, y / 144.0));
			}
		}
		completeLightColor /= count;
	}

	if (fTexID > 0)
	{
		// Get the textureColor
		int id = int(fTexID);
        color = texture(textures[id], fTexCoords) * completeLightColor * fColor;
	}
	else
	{
		color = fColor;
	}
}