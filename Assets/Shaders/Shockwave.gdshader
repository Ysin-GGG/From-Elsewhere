shader_type canvas_item;
uniform float Intensity = .1f;
uniform sampler2D Screen : hint_screen_texture, filter_linear;

void fragment() {
	float intensity = texture(TEXTURE, UV).g * Intensity;
	vec2 offset = (vec2(.5f) - UV) * 2.0f * intensity;
	COLOR = texture(Screen, SCREEN_UV + offset);
}