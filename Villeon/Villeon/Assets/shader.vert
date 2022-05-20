#version 330 core

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 color;
layout(location = 2) in vec2 texCoords;
layout(location = 3) in float texID;

uniform mat4 cameraMatrix;

out vec4 fColor;
out vec2 fTexCoords;
out float fTexID;

void main()
{
	fColor = color;
	fTexCoords = texCoords;
	fTexID = texID;

	gl_Position = position * cameraMatrix;
}