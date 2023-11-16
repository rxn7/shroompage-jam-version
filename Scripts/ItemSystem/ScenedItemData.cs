using Godot;

namespace Game.ItemSystem;

internal partial class ScenedItemData : ItemData {
    [Export] public PackedScene Scene { get; set; }

    public override Item Spawn() {
        return Scene.Instantiate() as Item;
    }
}