using Godot;

namespace Game.ItemSystem;

internal partial class SimpleItemData : ItemData {
    [Export] public float Mass { get; set; } = 1.0f;
    [Export] public Mesh Mesh { get; set; }
    [Export] public Shape3D CollisionShape { get; set; } // NOTE: Leave empty to auto generate CollisionShape from Mesh!

    public override Item Spawn() {
        Item item = new() {
            CollisionLayer = 1u << 30,
            Data = this,
            Mass = Mass,
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