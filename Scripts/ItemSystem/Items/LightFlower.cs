using Game.Player;

namespace Game.ItemSystem.Items;

internal partial class LightFlower : ConsumableItem {
	private const float BatteryTimeIncrease = 120.0f;

	public override void Consume(PlayerManager player) {
		player.Headlight.BatteryTimer += BatteryTimeIncrease;
	}
}
