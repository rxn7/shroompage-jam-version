using Game;
using Godot;

namespace Game;

internal partial class BatteryBarUI : BarUI {
	public override string Text => "Battery";
	public override Color FillColor => Colors.PaleGoldenrod;

	public override void _Process(double delta) {
		SetFill(GameManager.Singleton.Player.Headlight.BatteryPercentage);
	}
}
