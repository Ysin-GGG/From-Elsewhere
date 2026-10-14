shader_type canvas_item;
//render_mode blend_add;
uniform float Speed = 1.0f;
uniform float Threshold = .01f;uniform vec2 SlashOffset = vec2(.02f, .0f);
uniform vec2 NoiseOffset = vec2(.50f, .0f);
uniform sampler2D Noise : filter_linear, repeat_disable;
uniform sampler2D Screen : hint_screen_texture, filter_linear;

void fragment() {
	float noise = texture(Noise, fract(UV + vec2(TIME * Speed) * NoiseOffset)).r;
	float distortion = noise * COLOR.r;
	if (distortion < Threshold) distortion = .0f; 
	vec2 offset = SlashOffset * distortion;
	COLOR = texture(Screen, SCREEN_UV + offset);
}
