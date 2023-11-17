using Game.Player;
using Game.Utils;
using Godot;

namespace Game.ItemSystem;

internal abstract partial class ConsumableItem : Item {
	public ConsumableItemData ConsumableData => Data as ConsumableItemData;

	public override void Equip(PlayerManager player) {
		SoundManager.Play3D(player.GlobalPosition, ConsumableData.ConsumeSounds.GetRandomItem(), (float)GD.RandRange(0.9f, 1.1f));
		Consume(player);
		QueueFree();
	}

	public abstract void Consume(PlayerManager player);
}
