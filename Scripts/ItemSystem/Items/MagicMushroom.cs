using Game.ItemSystem;
using Game.Player;

internal partial class MagicMushroom : ConsumableItem {
    private const float HighLevelIncrease = 0.2f;
    private const float HealthIncrease = 0.1f;

    public override void Consume(PlayerManager player) {
        player.HighLevel += HighLevelIncrease;
        player.Health += HealthIncrease;
    }
}