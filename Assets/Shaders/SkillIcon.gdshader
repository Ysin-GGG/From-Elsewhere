shader_type canvas_item;

uniform sampler2D Frame : filter_linear, repeat_disable;
uniform vec2 FrameVelocity = vec2(.0f, .0f);
uniform vec4 Color : source_color = vec4(.3, .3, .3, 1.0);
uniform vec4 Flash : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float CooldownValue : hint_range(0, 1) = 1.0;
uniform float FlashValue : hint_range(0, 1) = 0.0;
uniform float Offset : hint_range(0, 1) = 0.0;
uniform float FrameAdd = 0.0;
uniform float FrameMul = 0.0;
uniform bool Clip = true;

void fragment() {
	float time = TIME;
	vec4 color = texture(TEXTURE, UV);
	vec2 pivot = vec2(.5, .5) - UV;
	float val = atan(pivot.x, -pivot.y) / TAU + 0.5;
	if (Clip) val = 1.0f - val;
	if (fract(val + Offset) < CooldownValue) COLOR *= Color;
	else
	{
		COLOR += Flash * FlashValue;
		vec2 offset = fract(UV + FrameVelocity * TIME);
		color = texture(Frame, offset);
		COLOR += COLOR * color * FrameMul;
	}
}