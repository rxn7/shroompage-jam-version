using Game.Player;
using Godot;

namespace Game;

internal partial class ScreenEffect : ColorRect {
	private const float MaxHighSaturation = 5.0f;
	private const float MaxHighContrast = 4.0f;
	private const float MaxHighNoiseIntensity = 0.02f;
	private const float MaxHighFisheyeStrength = 3.00f;
	private const float MaxHighHueAdjustSpeed = 5f;

	public PlayerManager Player { get; set; }

	private float m_HueAdjust = 0.0f;

	public override void _Process(double delta) {
		float targetHueAdjust = Mathf.IsZeroApprox(Player.HighLevel) ? 0.0f : m_HueAdjust + Player.HighLevel * MaxHighHueAdjustSpeed;
		m_HueAdjust = Mathf.Lerp(m_HueAdjust, targetHueAdjust, 0.5f * (float)delta);
		m_HueAdjust = (m_HueAdjust + 2 * Mathf.Pi) % (2 * Mathf.Pi);

		ShaderMaterial shader = Material as ShaderMaterial;
		shader.SetShaderParameter("saturation", Player.HighLevel * (MaxHighSaturation - 1.0) + 1.0);
		shader.SetShaderParameter("contrast", Player.HighLevel * (MaxHighContrast - 1.0) + 1.0);
		shader.SetShaderParameter("noiseIntensity", Player.HighLevel * (MaxHighNoiseIntensity - 0.001) + 0.001);
		shader.SetShaderParameter("fisheyeStrength", Player.HighLevel * (MaxHighFisheyeStrength - 1.0f) + 1.0f);
		shader.SetShaderParameter("hueAdjust", m_HueAdjust);
	}
}
