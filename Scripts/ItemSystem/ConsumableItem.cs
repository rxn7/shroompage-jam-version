using Game.Player;
using Game.Utils;
using Game.Story;
using Godot;

namespace Game.ItemSystem;

internal abstract partial class ConsumableItem : Item {
	public ConsumableItemData ConsumableData => Data as ConsumableItemData;

	public override void Equip(PlayerManager player) {
		StoryIntro intro = GameManager.Singleton.StoryIntro;

		Consume(player);
		QueueFree();

     	if (intro != null && intro.CollectedShroomCount < 6) {
            SoundManager.Play3D(player.GlobalPosition, ItemSpawner.BatteryMushroomData.ConsumeSounds.GetRandomItem(), (float)GD.RandRange(0.9f, 1.1f));
        } else {
            SoundManager.Play3D(player.GlobalPosition, ConsumableData.ConsumeSounds.GetRandomItem(), (float)GD.RandRange(0.9f, 1.1f));
        }
	}

	public abstract void Consume(PlayerManager player);
}
