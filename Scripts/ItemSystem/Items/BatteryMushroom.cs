using Game.Player;

namespace Game.ItemSystem.Items;

internal partial class BatteryMushroom : ConsumableItem {
	private const float BatteryTimeIncrease = 15.0f;

	public override void Consume(PlayerManager player) {
		player.Headlight.BatteryTimer += BatteryTimeIncrease;
	}
}
