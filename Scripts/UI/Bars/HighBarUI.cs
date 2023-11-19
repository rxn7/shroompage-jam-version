using Godot;

namespace Game;

internal partial class HighBarUI : BarUI {
	public override string Text => "High";
	public override Color FillColor => Colors.RebeccaPurple;

	public override void _Process(double delta) {
		SetFill(GameManager.Singleton.Player.HighLevel);
	}
}
