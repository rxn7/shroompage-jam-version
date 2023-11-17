
using System.Collections.Immutable;
using System.Linq;
using Game.Utils;
using Godot;

namespace Game.ItemSystem;

internal partial class HoldableItemData : ItemData {
    [ExportSubgroup("Audio")]
	[Export] public AudioStream[] PickupSounds = ResourceUtils.LoadAllAudioStreamsFromDirectory("res://Audio/Item/Pickup").ToArray();
	[Export] public AudioStream[] DropSounds = ResourceUtils.LoadAllAudioStreamsFromDirectory("res://Audio/Item/Drop").ToArray();

    [ExportSubgroup("Hand Slot")]
    [Export] public Vector3 HandSlotOffset { get; set; }
    [Export] public Vector3 HandSlotRotation { get; set; }

    [ExportSubgroup("Animations")]
    [Export] public StringName EquipAnimationName = "Equip";
    [Export] public StringName IdleAnimationName = "Idle";
    [Export] public StringName AttackAnimationName = "Attack";

    public override Item Spawn() {
        throw new System.NotImplementedException();
    }
}