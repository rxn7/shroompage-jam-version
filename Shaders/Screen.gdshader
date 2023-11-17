// adapted from:
// https://www.shadertoy.com/view/Ms3XWH

shader_type canvas_item;

uniform float noiseQuality: hint_range(0.1, 250,0.1) = 250;
uniform float noiseIntensity: hint_range(0, 0.05,0.001) = 0.001;
uniform float hueAdjust = 0.0;
uniform float saturation: hint_range(0.5, 2.0) = 1.0;
uniform float contrast: hint_range(0.5, 2.0) = 1.0;
uniform float fisheyeStrength = 0.0f;
uniform int resX = 860;
uniform int resY = 640;
uniform vec3 rgb255 = vec3(0.100392156862, 0.100392156862, 0.100392156862);
uniform sampler2D screenTexture : hint_screen_texture, repeat_disable, filter_nearest;

float rand(vec2 co) {
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

vec3 hueShift(vec3 color) {
    const vec3  kRGBToYPrime = vec3 (0.299, 0.587, 0.114);
    const vec3  kRGBToI      = vec3 (0.596, -0.275, -0.321);
    const vec3  kRGBToQ      = vec3 (0.212, -0.523, 0.311);

    const vec3  kYIQToR     = vec3 (1.0, 0.956, 0.621);
    const vec3  kYIQToG     = vec3 (1.0, -0.272, -0.647);
    const vec3  kYIQToB     = vec3 (1.0, -1.107, 1.704);

    float   YPrime  = dot (color, kRGBToYPrime);
    float   I       = dot (color, kRGBToI);
    float   Q       = dot (color, kRGBToQ);
    float   hue     = atan (Q, I);
    float   chroma  = sqrt (I * I + Q * Q);

    hue += hueAdjust;

    Q = chroma * sin (hue);
    I = chroma * cos (hue);

    vec3    yIQ   = vec3 (YPrime, I, Q);

    return vec3( dot (yIQ, kYIQToR), dot (yIQ, kYIQToG), dot (yIQ, kYIQToB) );
}

vec2 fisheye(vec2 uv) {
    vec2 d = uv - 0.5;
    float r = length(d);
    float theta = atan(d.y, d.x);
    float rf = pow(r, fisheyeStrength) / pow(0.5, fisheyeStrength - 1.0);
    return vec2(0.5) + rf * normalize(d);
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
    uv = fisheye(uv);
	float uvY = uv.y;

    uvY *= noiseQuality;
    uvY = float(int(uvY)) * (1.0 / noiseQuality);

    float noise = rand(vec2(TIME* 0.00001, uvY));
    uv.x += noise * noiseIntensity;
	
	COLOR = vec4(pixelate(uv), 1.0);

    // Saturation
    vec3 greyscale = vec3(COLOR.r + COLOR.g + COLOR.b) / 3.0;
    COLOR.rgb = mix(greyscale, COLOR.rgb, saturation);
    
    // Contrast
    float midpoint = pow(0.5, 2.2);
    COLOR.rgb = (COLOR.rgb - vec3(midpoint)) * contrast + midpoint;

    // Hue shift
    COLOR.rgb = hueShift(COLOR.rgb);
}