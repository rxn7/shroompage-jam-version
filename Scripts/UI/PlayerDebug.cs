using Godot;
using System.Text;
using Game.Player;

namespace Game.UI;

internal partial class PlayerDebug : Label {
	public PlayerManager Player { get; set; }
	private readonly StringBuilder m_StringBuilder = new();

	public override void _PhysicsProcess(double dt) {
		m_StringBuilder.Clear();

		m_StringBuilder.AppendLine($"Controller:")
					   .AppendLine($"  MovementState: {Player.Controller.MovementState}")
					   .AppendLine($"  Surface: {Player.Controller.SurfaceMaterial}")
					   .AppendLine($"  Input: {Player.Controller.Inputs.Move.ToString("0.00")}")
					   .AppendLine($"  Direction: {Player.Controller.Direction.ToString("0.00")}")
					   .AppendLine($"  MoveSpeed: {Player.Controller.MoveSpeed:0.00}")
					   .AppendLine($"  TargetSpeed: {Player.Controller.TargetMoveSpeed:0.00}")
					   .AppendLine($"  Velocity: {Player.GetRealVelocity().Length():0.00}")
					   .AppendLine($"  Bob: {Player.Bobbing.Bob.ToString("0.00")}")
					   .AppendLine($"  Height: {Player.Controller.Height:0.00}")

					   .AppendLine($"\nHead:")
					   .AppendLine($"  Pitch: {Player.Head.Pitch:0.00}")
					   .AppendLine($"  StrafeRoll: {Player.Head.StrafeRoll:0.00}")

					   .AppendLine($"\nViewmodel:")
					   .AppendLine($"  Sway: {Player.Viewmodel.Sway.ToString("0.00")}")
					   .AppendLine($"\nBattery: {Player.Headlight.BatteryPercentage*100:0}%")
					   .AppendLine($"High Level: {Player.HighLevel*100:0}%")
					   .AppendLine($"\nEnemy count: {GameManager.Singleton.GetEnemyCount()}");

		Text = m_StringBuilder.ToString();
	}
}
