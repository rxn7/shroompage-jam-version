using Godot;

namespace Game.Player; 

internal partial class PlayerViewmodelAnimationPlayer : AnimationPlayer {
    public PlayerViewmodel Viewmodel { get; set; }
    
    public override void _Ready() {
        base._Ready();
        AnimationFinished += OnAnimationFinished;
    }

    private void OnAnimationFinished(StringName name) {
        Play("FistsIdle");
    }
}