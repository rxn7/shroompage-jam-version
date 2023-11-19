using Godot;
using Game.Surface;
using Game.Utils;

namespace Game;

internal partial class Initializer : Node {
	public override void _EnterTree() {
		GD.Randomize();
		FootstepManager.Init();
		SoundManager.Init(GetTree());
	}
}
