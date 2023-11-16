// adapted from:
// https://www.shadertoy.com/view/Ms3XWH

shader_type canvas_item;

uniform float noiseQuality: hint_range(0.1, 250,0.1) = 250;
uniform float noiseIntensity: hint_range(0, 0.05,0.001) = 0.001;
uniform float offsetIntensity: hint_range(0, 0.05,0.0001) = 0.05;
uniform float colorOffsetIntensity: hint_range(0.1, 1.5,0.001) = 0.5;

uniform sampler2D screenTexture : hint_screen_texture, repeat_disable, filter_nearest;

uniform int resX = 860;
uniform int resY = 640;
uniform vec3 rgb255 = vec3(0.100392156862, 0.100392156862, 0.100392156862);

float rand(vec2 co) {
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

vec3 pixelate(vec2 uv) {
	float uvX = uv.x - mod(uv.x * float(resX), 1) / float(resX);
	float uvY = uv.y - mod(uv.y * float(resY), 1) / float(resY);
	vec2 grid_uv = vec2(uvX, uvY);
	
	vec3 col = texture(screenTexture, grid_uv).rgb;
	
	if(col.r < 1.0 && col.g < 1.0 && col.b < 1.0) {
		vec3 remainder = mod(col.rgb, rgb255);
		col.rgb = col.rgb - remainder;
	}
	
	return col;
}

void fragment() {
	vec2 uv = FRAGCOORD.xy / (1.0 / SCREEN_PIXEL_SIZE).xy;
	float uvY = uv.y;

    uvY *= noiseQuality;
    uvY = float(int(uvY)) * (1.0 / noiseQuality);

    float noise = rand(vec2(TIME* 0.00001, uvY));
    uv.x += noise * noiseIntensity;
	
	COLOR = vec4(pixelate(uv), 1.0);
}