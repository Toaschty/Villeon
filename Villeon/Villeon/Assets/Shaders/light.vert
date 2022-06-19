#version 410 core


layout(location = 0) in vec4 position;
layout(location = 1) in vec4 color;
layout(location = 2) in vec2 texCoords;
layout(location = 3) in float texID;

out vec4 fColor;
out vec2 fTexCoords;
out float fTexID;
out vec3 fPosition;
uniform mat4 cameraMatrix;
uniform mat4 screenMatrix;

void main()
{
    fColor = color;
	fPosition = position.xyz;
	fTexCoords = texCoords;
	fTexID = texID;

	// Screen Layers: -0, -1, -2, -3, -4, -5, -6, -7
	if (position.z >= -7 )
	{
		gl_Position = position * screenMatrix;
	}

	// everything after the screen layers uses the camera
	if (position.z < -7)
	{
		gl_Position = position * cameraMatrix;
	}
}