#version 410 core

uniform float uTime;
out vec4 outColor;

in vec4 fColor;
in vec2 fTexCoords;
in float fTexID;

uniform sampler2D textures[8];

void main()
{
	float time = uTime * 0.4;
	vec2 uv = noise2(time);
	vec2 t = fTexCoords * uv;
    outColor = vec4(t.x, t.y, 0, 1);
}

