using Godot;

namespace Game.ItemSystem; 

internal partial class ItemSpawner : Node3D {
	public static readonly ConsumableItemData MagicMushroomData = GD.Load<ConsumableItemData>("res://Items/MagicMushroom.tres");
	public static readonly ConsumableItemData BatteryMushroomData = GD.Load<ConsumableItemData>("res://Items/BatteryMushroom.tres");

	[Export(PropertyHint.ResourceType, "ItemData")] private ItemData m_ItemData;

	public override void _Ready() {
		AddChild(m_ItemData?.Spawn());
	}
}
