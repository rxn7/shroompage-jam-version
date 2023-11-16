using Godot;

namespace Game.ItemSystem; 

internal partial class ItemSpawner : Node3D {
	[Export(PropertyHint.ResourceType, "ItemData")] private ItemData m_ItemData;

	public override void _Ready() {
		AddChild(m_ItemData?.Spawn());
	}
}
