using Godot;

namespace Game.UI;

internal partial class RespawnButton : Button {
	public override void _Ready() {
		Input.MouseMode = Input.MouseModeEnum.Visible;
		Pressed += Respawn;
	}

	private void Respawn() {
		GetTree().ChangeSceneToFile("res://Scenes/Game.tscn");
	}
}
