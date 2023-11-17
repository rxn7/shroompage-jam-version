using Godot;

namespace Game.ItemSystem;

internal partial class ConsumableItemData : ItemData {
	[Export] public Script Script { get; private set; } = GD.Load<Script>("res://Scripts/ItemSystem/Item.cs");

	[ExportSubgroup("Audio")]
	[Export] public AudioStream[] ConsumeSounds = new AudioStream[0];

	public override ConsumableItem Spawn() {
		RigidBody3D itemBody = new();

		ulong itemId = itemBody.GetInstanceId();
		itemBody.SetScript(Script);
		
		ConsumableItem item = InstanceFromId(itemId) as ConsumableItem;

		item.CollisionLayer = 1u << 30;
		item.CollisionMask = 1u << 0 | 1u << 30;
		item.Data = this;
		item.Mass = Mass;

		MeshInstance3D meshInstance = new() { Mesh = Mesh, };
		item.AddChild(meshInstance);

		CollisionShape3D shape = new() { Shape = CollisionShape };
		item.AddChild(shape);

		return item;
	}
}
