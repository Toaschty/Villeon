#version 410 core

layout (location = 0) in vec4 aPos;

uniform mat4 cameraMatrix;

void main()
{
	gl_Position = aPos * cameraMatrix;
}