#version 410

out vec4 FragColor;

layout (location = 1) in vec4 v_vertex_color;

void main()
{
	//FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);

	FragColor =  v_vertex_color;
}