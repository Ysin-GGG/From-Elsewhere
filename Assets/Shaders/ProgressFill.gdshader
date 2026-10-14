shader_type canvas_item;

uniform vec4 Color : source_color = vec4(1.000000, 1.000000, 1.000000, 1.000000);
uniform float Value : hint_range(0, 1) = .5f;

void fragment() {
	COLOR *= Color;
	if (UV.x > Value) COLOR.a = 0.0;
}