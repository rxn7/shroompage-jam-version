using Game;
using Game.Player;
using Godot;

internal partial class HealthBarUI : BarUI {
    public override string Text => "Health";
    public override Color FillColor => Colors.PaleVioletRed;

    public override void _Ready() {
        base._Ready();
        GameManager.Singleton.Player.OnHealthChanged += OnHealthChanged;
    }

    public override void _ExitTree() {
        base._ExitTree();
        GameManager.Singleton.Player.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float health) {
        SetFill(health / PlayerManager.MaxHealth);
    }
}