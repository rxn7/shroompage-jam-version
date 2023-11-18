using System.Text;
using Game.Player;
using Godot;

namespace Game;

internal partial class PlayerStatsUI : Label {
	private PlayerManager m_Player;
	private StringBuilder m_StringBuilder = new();

	public override void _Ready() {
		m_Player = GetParent().GetParent<PlayerManager>();
	}

	public override void _PhysicsProcess(double delta) {
		m_StringBuilder.Clear();
		m_StringBuilder.AppendLine($"Health: {m_Player.Health}");
		m_StringBuilder.AppendLine($"High: {m_Player.HighLevel * 100:0}%");
		m_StringBuilder.AppendLine($"Battery: {m_Player.Headlight.BatteryPercentage * 100:0}%");

		Text = m_StringBuilder.ToString();
	}
}
