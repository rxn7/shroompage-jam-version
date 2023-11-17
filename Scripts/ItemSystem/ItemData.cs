using Godot;

namespace Game.ItemSystem;

[Tool]
internal abstract partial class ItemData : Resource {
    [Export] public StringName Name { get; set; }

    [ExportSubgroup("Hand Slot")]
    [Export] public Vector3 HandSlotOffset { get; set; }
    [Export] public Vector3 HandSlotRotation { get; set; }

    [ExportSubgroup("Animations")]
    [Export] public StringName EquipAnimationName = "Equip";
    [Export] public StringName IdleAnimationName = "Idle";
    [Export] public StringName AttackAnimationName = "Attack";

    public abstract Item Spawn();
}