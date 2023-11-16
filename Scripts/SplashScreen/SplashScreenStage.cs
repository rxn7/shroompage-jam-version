using Godot;

namespace Game.SplashScreen;

public partial class SplashScreenStage : Control {
    [Export] public float FadeTime { get; private set; } = 0.25f ;
	[Export] public float DisplayTime { get; private set; }= 0.75f;
}