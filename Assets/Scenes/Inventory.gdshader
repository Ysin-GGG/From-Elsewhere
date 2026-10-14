shader_type canvas_item;
uniform sampler2D Screen : hint_screen_texture, filter_linear;
uniform sampler2D Border : repeat_disable, filter_nearest;
uniform sampler2D Background : repeat_disable, filter_nearest;
uniform sampler2D Frontground : repeat_disable, filter_nearest;
uniform vec4 BorderColor : source_color = vec4(1.0, 0.0, 0.0, 1.0);
uniform vec4 BackgroundColor : source_color = vec4(0.0, 1.0, 0.0, 1.0);
uniform vec4 FrontgroundColor : source_color = vec4(0.0, 0.0, 1.0, 1.0);

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	if (COLOR.r == 1.0) COLOR = texture(Border, UV) * BorderColor;
	else if (COLOR.g == 1.0) COLOR = texture(Background, UV) * BackgroundColor;
	else if (COLOR.b == 1.0) COLOR = texture(Frontground, UV) * FrontgroundColor;
}