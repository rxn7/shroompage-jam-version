using Godot;
using Game.Surface;
using Game.Utils;

namespace Game;

internal partial class Initializer : Node {
	public override void _EnterTree() {
		GD.Randomize();
		SurfaceManager.Init();
		SoundManager.Init(GetTree());
	}
}
