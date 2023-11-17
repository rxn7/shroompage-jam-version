using Game.ItemSystem;
using Game.Player;

internal partial class MagicMushroom : ConsumableItem {
    public override void Consume(PlayerManager player) {
        player.HighLevel += 0.1f;
        // TODO: Increase health
    }
}