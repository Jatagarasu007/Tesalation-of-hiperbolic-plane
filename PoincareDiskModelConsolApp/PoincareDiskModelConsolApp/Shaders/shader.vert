#version 410

layout (location = 0) in vec3 position;
layout (location = 1) in vec4 color ;

out vec4 v_vertex_color; 

void main()
{
    gl_Position = vec4(position, 1.0);
    v_vertex_color = color;
    //vertexPositionComputed_attribute1 = vec4(vertexPosition_attribute1, 1.0);
}