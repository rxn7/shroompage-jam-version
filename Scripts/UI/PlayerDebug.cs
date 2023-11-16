using Godot;
using System.Text;
using Game.Player;

namespace Game.UI;

internal partial class PlayerDebug : Label {
	[Export] private PlayerManager m_Player;
	private readonly StringBuilder m_StringBuilder = new();

	public override void _PhysicsProcess(double dt) {
		m_StringBuilder.Clear();

		m_StringBuilder.AppendLine($"Controller:")
					   .AppendLine($"    MovementState: {m_Player.Controller.MovementState}")
					   .AppendLine($"    Surface: {m_Player.Controller.SurfaceMaterial}")
					   .AppendLine($"    Input: {m_Player.Controller.Inputs.Move.ToString("0.00")}")
					   .AppendLine($"    Direction: {m_Player.Controller.Direction.ToString("0.00")}")
					   .AppendLine($"    MoveSpeed: {m_Player.Controller.MoveSpeed:0.00}")
					   .AppendLine($"    TargetSpeed: {m_Player.Controller.TargetMoveSpeed:0.00}")
					   .AppendLine($"    Velocity: {m_Player.GetRealVelocity().Length():0.00}")
					   .AppendLine($"    Bob: {m_Player.Bobbing.Bob.ToString("0.00")}")
					   .AppendLine($"    Height: {m_Player.Controller.Height:0.00}")

					   .AppendLine($"\nHead:")
					   .AppendLine($"    Pitch: {m_Player.Head.Pitch:0.00}")
					   .AppendLine($"    StrafeRoll: {m_Player.Head.StrafeRoll:0.00}")

					   .AppendLine($"\nViewmodel:")
					   .AppendLine($"    Sway: {m_Player.Viewmodel.Sway.ToString("0.00")}");

		Text = m_StringBuilder.ToString();
	}
}
