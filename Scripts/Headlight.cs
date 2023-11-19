using Godot;

namespace Game;

public partial class Headlight : SpotLight3D {
	private const float MaxBatteryTime = 300.0f;
	private const float MaxLightEnergy = 1.5f;
	private const float MinLightEnergy = 0.2f;
	private const float MaxLightRange = 30.0f;
	private const float MinLightRange = 5.0f;
	private const float MaxLightAngle = 80.0f;
	private const float MinLightAngle = 30.0f;

	public float BatteryPercentage => m_BatteryTimer / MaxBatteryTime;

	public float BatteryTimer { 
		get => m_BatteryTimer;
		set {
			m_BatteryTimer = Mathf.Clamp(value, 0, MaxBatteryTime);
			float batteryPercentage = BatteryPercentage;
			LightIndirectEnergy = LightEnergy = batteryPercentage * (MaxLightEnergy - MinLightEnergy) + MinLightEnergy;
			SpotAngle = batteryPercentage * (MaxLightAngle - MinLightAngle) + MinLightAngle;
			SpotRange = batteryPercentage * (MaxLightRange - MinLightRange) + MinLightRange;
		}
	}
	private float m_BatteryTimer;

	public Headlight() {
		m_BatteryTimer = MaxBatteryTime;
	}

	public override void _Process(double delta) {
        if(Visible)
            BatteryTimer -= (float)delta;
	}
}
