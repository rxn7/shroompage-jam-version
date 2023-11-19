using Game;
using Game.ItemSystem;
using Game.Player;
using Game.Story;

internal partial class MagicMushroom : ConsumableItem {
    private const float HighLevelIncrease = 0.2f;
    private const float HealthIncrease = 20f;

    public override void Consume(PlayerManager player) {
        StoryIntro intro = GameManager.Singleton.StoryIntro;
        if (intro != null) {
            // lazy
            intro.CollectShroom();
            if (intro.DisableShroomEffects) return;
        }   

        player.Health += HealthIncrease;
        player.HighLevel += HighLevelIncrease;
    }
}