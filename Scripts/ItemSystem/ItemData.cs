using Godot;

namespace Game.ItemSystem;

[Tool]
internal abstract partial class ItemData : Resource {
    [Export] public StringName Name { get; set; }
    [Export] public Vector3 HandSlotOffset { get; set; }
    [Export] public Vector3 HandSlotRotation { get; set; }

    public abstract Item Spawn();
}