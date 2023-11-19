using Game.Player;
using Game.Utils;
using Godot;

namespace Game.ItemSystem;

internal partial class HoldableItem : Item {
	public HoldableItemData HoldableData => Data as HoldableItemData;

	public override void Equip(PlayerManager player) {
		base.Equip(player);
		ProcessMode = ProcessModeEnum.Disabled;
		SoundManager.Play3D(player.GlobalPosition, HoldableData.PickupSounds.GetRandomItem(), (float)GD.RandRange(0.9f, 1.1f));
		player.Viewmodel.AttachToHandSlot(this, HoldableData.HandSlotOffset, HoldableData.HandSlotRotation);
		player.Viewmodel.PlayEquipAnimation();
	}

	public void Drop(Vector3 playerVelocity, Vector3 forward) {
		ApplyHighlightShader();
		SoundManager.Play3D(GlobalPosition, HoldableData.DropSounds.GetRandomItem(), (float)GD.RandRange(0.9f, 1.1f));
		ProcessMode = ProcessModeEnum.Inherit;
		Reparent(GetTree().CurrentScene);
		ApplyImpulse(forward * DropForce + playerVelocity);
	}
}
