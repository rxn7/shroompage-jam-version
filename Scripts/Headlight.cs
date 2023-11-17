using Godot;

namespace Game;

public partial class Headlight : SpotLight3D {
	private const float MaxBatteryTime = 5.0f;
	private const float MaxLightEnergy = 1.0f;
	private const float MinLightEnergy = 0.1f;
	private const float MaxLightAngle = 35.0f;
	private const float MinLightAngle = 15.0f;

	public float BatteryPercentage => m_BatteryTimer / MaxBatteryTime;

	public float BatteryTimer { 
		get => m_BatteryTimer;
		set {
			m_BatteryTimer = Mathf.Clamp(value, 0, MaxBatteryTime);
			float batteryPercentage = BatteryPercentage;
			LightIndirectEnergy = LightEnergy = batteryPercentage * (MaxLightEnergy - MinLightEnergy) + MinLightEnergy;
			SpotAngle = batteryPercentage * (MaxLightAngle - MinLightAngle) + MinLightAngle;
		}
	}
	private float m_BatteryTimer;

	public Headlight() {
		m_BatteryTimer = MaxBatteryTime;
	}

	public override void _Process(double delta) {
		BatteryTimer -= (float)delta;
	}
}
