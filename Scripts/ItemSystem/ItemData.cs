using Godot;

namespace Game.ItemSystem;

[Tool]
internal partial class ItemData : Resource {
    [Export] public StringName Name { get; set; }
    [Export] public float Mass { get; set; } = 1.0f;
    [Export] public Vector3 HandSlotOffset { get; set; }
    [Export] public Vector3 HandSlotRotation { get; set; }
    [Export] public Mesh Mesh { get; set; }
    [Export] public Shape3D CollisionShape { get; set; } // NOTE: Leave empty to auto generate CollisionShape from Mesh!

    private static readonly Resource ItemScriptResource = GD.Load("res://Scripts/ItemSystem/Item.cs");
    protected virtual Resource ScriptResource => ItemScriptResource;

    public Item Spawn() {
        Item item = new() {
            CollisionLayer = 1u << 30,
            Data = this,
        };

        MeshInstance3D meshInstance = new() {
            Mesh = Mesh,
        };
        item.AddChild(meshInstance);

        CollisionShape3D shape = new() {
            Shape = CollisionShape ?? Mesh.CreateConvexShape(),
        };
        item.AddChild(shape);
        
        return item;
    }
}