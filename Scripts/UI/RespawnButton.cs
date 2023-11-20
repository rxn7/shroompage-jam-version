using Godot;
using Game.Story;

namespace Game.UI;

internal partial class RespawnButton : Button {
	public override void _Ready() {
		Input.MouseMode = Input.MouseModeEnum.Visible;
		Pressed += Respawn;
	}

	private void Respawn() {
		Game.Story.StoryIntroDebug.DisableIntro = true;
		GetTree().ChangeSceneToFile("res://Scenes/Game.tscn");
	}
}
