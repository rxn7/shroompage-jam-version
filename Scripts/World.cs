using Godot;
using Game.Player;

namespace Game; 

internal partial class World : Node {
	public static World Instance { get; private set; }
	
	[Export] public PlayerManager Player { get; private set; }

	public override void _EnterTree() {
		Instance = this;
	}
}
