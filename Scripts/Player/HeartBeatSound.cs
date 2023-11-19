using Game.Player;
using Godot;

namespace Game;

internal partial class HeartBeatSound : AudioStreamPlayer {
	private PlayerManager m_Player;
	private const float HealthThreshold = 30.0f;
	private const float MinVolume = -1.0f;
	private const float MaxVolume = 10.0f;

	public override void _Ready() {
		m_Player = GetParent<PlayerManager>();
		Play(0);
		UpdateVolume();

		m_Player.OnDamage += (float dmg) => UpdateVolume();
	}

	private void UpdateVolume() {
		VolumeDb = m_Player.Health > 30 ? -100 : Mathf.Lerp(MaxVolume, MinVolume, m_Player.Health / HealthThreshold);
	}
}
