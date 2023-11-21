using System;
using Game.ItemSystem;
using Game.Player;

internal partial class MagicMushroom : ConsumableItem {
    public static bool DisableEffects = false;
    public static Action<MagicMushroom> Consumed;

    private const float HighLevelIncrease = 0.2f;
    private const float HealthIncrease = 20f;

    public override void Consume(PlayerManager player) {
        Consumed?.Invoke(this);

        if(!DisableEffects) {
            player.Health += HealthIncrease;
            player.HighLevel += HighLevelIncrease;
        }
    }
}